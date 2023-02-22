using System;
using System.Linq;
using UnityEngine;
using UI.Li.Utils;
using UI.Li.Internal;
using JetBrains.Annotations;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Collections.Concurrent;

#if UNITY_EDITOR // for reference listing
using System.Collections.ObjectModel;
#endif

namespace UI.Li
{
    public class CompositionContext: IDisposable
    {
        /// <summary>
        /// Convenient update batch scope guard. <seealso cref="CompositionContext.BatchOperations"/>
        /// </summary>
        [PublicAPI] public struct BatchScope: IDisposable
        {
            [NotNull] private readonly CompositionContext context;
            
            public BatchScope([NotNull] CompositionContext ctx)
            {
                context = ctx;
                ctx.EnterBatchScope();
            }
            public void Dispose() => context.LeaveBatchScope();
        }
     
        /// <summary>
        /// Allows to specify how state differences for sub-compositions will be handled. The default is <see cref="CompositionContext.RecompositionStrategy.Override"/>.
        /// </summary>
        [PublicAPI] public enum RecompositionStrategy
        {
            /// <summary>
            /// Override every non matching or unnamed entry.
            /// </summary>
            Override,
            /// <summary>
            /// Try to reorder previous sub-compositions to match current layout. Introduces overhead, use <see cref="CompositionContext.RecompositionStrategy.Override"/> when possible.
            /// </summary>
            Reorder
        }
        
        private struct Frame: IDisposable
        {
            public class FrameEntry: IDisposable
            {
                public readonly int Id;
                public readonly int NestingLevel;
                [NotNull] public IComposition Composition;
                public bool Dirty;
                public RemapHelper<int> Reordering;
                public VisualElement PreviouslyRendered;

                public FrameEntry(int id, int nestingLevel, [NotNull] IComposition composition)
                {
                    Id = id;
                    NestingLevel = nestingLevel;
                    Composition = composition;
                    Dirty = true;
                    PreviouslyRendered = null;
                }

                public void CleanupReordering() => Reordering = null;

                public void Dispose()
                {
                    Composition.Dispose();
                    CleanupReordering();
                }
            }

            public class FrameCallback: IDisposable
            {
                public readonly Func<Action> OnUpdate;
                public Action OnDispose;
                
                public void Dispose() => OnDispose?.Invoke();

                public FrameCallback([NotNull] Func<Action> onUpdate)
                {
                    OnUpdate = onUpdate;
                    OnDispose = OnUpdate();
                }
            }
            
            public enum FrameType
            {
                [UsedImplicitly] Empty,
                Entry,
                Field,
                Callback
            }

            public readonly FrameType Type;
            public readonly IMutableValue Field;
            public readonly FrameEntry Entry;
            public readonly FrameCallback Callback;
            
            public Frame([NotNull] IComposition composition, int nestingLevel, int entryId)
            {
                Type = FrameType.Entry;
                Field = null;
                Entry = new FrameEntry(entryId, nestingLevel, composition);
                Callback = null;
            }

            public Frame(IMutableValue field)
            {
                Type = FrameType.Field;
                Field = field;
                Entry = null;
                Callback = null;
            }

            public Frame(Func<Action> onUpdate)
            {
                Type = FrameType.Callback;
                Field = null;
                Entry = null;
                Callback = new FrameCallback(onUpdate);
            }

            public void Dispose()
            {
                Entry?.Composition.Dispose();
                Field?.Dispose();
                Callback?.Dispose();
            }
        }

#if UNITY_EDITOR // for reference listing
        public class CompositionNode
        {
            public readonly string Name;
            public readonly ReadOnlyCollection<IMutableValue> Values;
            public readonly ReadOnlyCollection<CompositionNode> Children;

            public CompositionNode(string name, List<IMutableValue> values, List<CompositionNode> children)
            {
                Name = name;
                Values = values.AsReadOnly();
                Children = children.AsReadOnly();
            }
        }
        
        private class TmpNode
        {
            private readonly string name;
            private readonly List<IMutableValue> values = new();
            private readonly List<TmpNode> children = new ();

            public TmpNode(string name)
            {
                this.name = name;
            }

            public void AddValue(IMutableValue value) => values.Add(value);
            public void AddChild(TmpNode child) => children.Add(child);

            public CompositionNode GetNode() => new CompositionNode(name, values, children.Select(n => n.GetNode()).ToList());
        }
        
        [NotNull] public static IEnumerable<CompositionContext> Instances => instances.Select(r => r.TryGetTarget(out var instance) ? instance : null).Where(i => i != null);
        public static event Action OnInstanceListChanged;
#endif
        
        [NotNull] [PublicAPI] public readonly string Name;

#if UNITY_EDITOR // for reference listing
        private static readonly ConcurrentQueue<Action> syncQueue = new();
#endif
        
        private static readonly HashSet<WeakReference<CompositionContext>> instances = new();

        private readonly GapBuffer<Frame> frames = new ();
        private int framePointer;
        private bool isFirstRender;

        private readonly Stack<Frame.FrameEntry> entryStack = new();

        private bool dirty;
        
        private int batchScopeLevel;
        private int nextEntryId;
        private bool nextEntryPreventOverride;

#if UNITY_EDITOR // for reference listing
        static CompositionContext()
        {
            UnityEditor.EditorApplication.update += () =>
            {
                while (syncQueue.TryDequeue(out var action))
                    action();
            };
        }
        
        ~CompositionContext()
        {
            UnregisterInstance(this);
        }
#endif
        
        public CompositionContext(string name = "Unnamed")
        {
            Name = name;
            RegisterInstance(this);
        }

        /// <summary>
        /// Enters update batching scope. <seealso cref="BatchOperations"/>
        /// </summary>
        [PublicAPI] public void EnterBatchScope() => ++batchScopeLevel;

        /// <summary>
        /// Leaves update batching scope. <seealso cref="BatchOperations"/>
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when there is no scope to leave.</exception>
        [PublicAPI] public void LeaveBatchScope()
        {
            if (batchScopeLevel == 0)
                throw new InvalidOperationException("No batch scope to leave");
            
            --batchScopeLevel;

            if (batchScopeLevel == 0 && dirty)
                MakeDirty();
        }
        
        /// <summary>
        /// Batches state updates to prevent excessive recompositions.
        /// </summary>
        /// <example>
        /// To batch operations inside any scope, place this line at the beginning of the given scope.
        /// <code>
        /// using var _ = ctx.BatchOperations();
        /// </code>
        /// </example>
        /// <returns>Batch scope guard</returns>
        [PublicAPI] public BatchScope BatchOperations() => new(this);
        
        /// <summary>
        /// Sets composition entry id for next recomposition. 
        /// </summary>
        /// <remarks>Difficult to use outside custom composition implementation. To give id to composition use <see cref="CompositionUtils.WithId"/></remarks>
        /// <param name="entryId">Id for next composition</param>
        [PublicAPI] public void SetNextEntryId(int entryId = 0) => nextEntryId = entryId;

        /// <summary>
        /// Forces preferring updates over overrides in the next recomposition. 
        /// </summary>
        /// <remarks>Difficult to use outside custom composition implementation.</remarks>
        [PublicAPI] public void PreventNextEntryOverride() => nextEntryPreventOverride = true;
        
        /// <summary>
        /// Starts composing given composition.
        /// </summary>
        /// <remarks>Should be called at the beginning of <see cref="IComposition.Recompose"/> in custom composition implementation.</remarks>
        /// <returns>Previously rendered element for given composition.</returns>
        [PublicAPI]
        public VisualElement StartFrame([NotNull] IComposition composition, RecompositionStrategy strategy = RecompositionStrategy.Override)
        {
            int currentNestingLevel = entryStack.TryPeek(out var lastEntry) ? lastEntry.NestingLevel + 1 : 0;
            
            isFirstRender = SetupFrame(nextEntryId, currentNestingLevel, nextEntryPreventOverride, lastEntry?.Reordering);
            nextEntryPreventOverride = false;
            nextEntryId = 0;

            VisualElement previouslyRendered = null;

            Frame.FrameEntry currentEntry;
            
            if (isFirstRender)
            {
                InsertAtPointer(new Frame(
                    entryId: nextEntryId,
                    nestingLevel: currentNestingLevel,
                    composition: composition
                ));
                
                currentEntry = frames[framePointer - 1].Entry;
                
                composition.OnRender += v => currentEntry.PreviouslyRendered = v;
            }
            else
            {
                var frame = GetAtPointer();

                currentEntry = frame.Entry;

                if (currentEntry.Composition != composition)
                {
                    currentEntry.Composition.Dispose();
                    currentEntry.Composition = composition;
                    composition.OnRender += v => currentEntry.PreviouslyRendered = v;
                }
                previouslyRendered = frame.Entry.PreviouslyRendered;
            }
            
            entryStack.Push(currentEntry);
            currentEntry.Dirty = false;

            if (strategy == RecompositionStrategy.Reorder)
            {
                int offset = framePointer - 1;
                
                Frame.FrameEntry activeEntry = null;
                int activeSize = 0;
                
                var entries = new List<(int id, int size)>();
                
                for (int i = framePointer; i < frames.Count; ++i)
                {
                    activeSize++;
                    var frame = frames[i];

                    if (activeEntry == null)
                        ++offset;
                    
                    if (frame.Type != Frame.FrameType.Entry)
                        continue;

                    var entry = frame.Entry;

                    if (entry.NestingLevel > currentNestingLevel + 1) continue;
                    
                    if (activeEntry != null)
                    {
                        entries.Add((activeEntry.Id, activeSize));
                    }
                    
                    activeEntry = entry;
                    activeSize = 0;

                    if (entry.NestingLevel > currentNestingLevel) continue;
                    
                    activeSize--;
                    activeEntry = null;
                    break;
                }
                
                if (activeEntry != null)
                    entries.Add((activeEntry.Id, activeSize));

                if (entries.Count > 0)
                {
                    currentEntry.Reordering = new(offset, entries);
                }
            }

            SetNextEntryId();

            return previouslyRendered;
        }

        /// <summary>
        /// Ends composing current composition.
        /// </summary>
        /// <remarks>Should be called at the end of <see cref="IComposition.Recompose"/> in custom composition implementation.</remarks>
        [PublicAPI]
        public void EndFrame()
        {
            if (entryStack.Count == 0)
                throw new InvalidOperationException("EndFrame called more times than StartFrame");
            
            int currentNestingLevel = entryStack.TryPeek(out var currentEntry) ? currentEntry.NestingLevel : 0;
            
            ClearAllNested(currentNestingLevel, false);
            entryStack.Pop().CleanupReordering();
        }

        /// <summary>
        /// Adds given state value to composition state.
        /// </summary>
        /// <seealso cref="Remember{T}"/>
        /// <param name="value">state value to add</param>
        /// <typeparam name="T">type of state value, must implement <see cref="IMutableValue"/></typeparam>
        /// <returns>Returns current state of the value</returns>
        /// <exception cref="InvalidOperationException">Thrown when different invocation order detected</exception>
        [PublicAPI]
        public T Use<T>(T value) where T: class, IMutableValue => Use(() => value);

        /// <summary>
        /// Adds given state value to current composition state.
        /// </summary>
        /// <seealso cref="RememberF{T}"/>
        /// <param name="factory">factory of state value to add</param>
        /// <typeparam name="T">type of state value, must implement <see cref="IMutableValue"/></typeparam>
        /// <returns>Returns current state of the value</returns>
        /// <exception cref="InvalidOperationException">Thrown when different invocation order detected</exception>
        [PublicAPI]
        public T Use<T>([NotNull] Func<T> factory) where T : class, IMutableValue
        {
            Debug.Assert(factory != null);

            if (!entryStack.TryPeek(out var currentEntry))
                throw new InvalidOperationException("Using state outside composable function");

            if (isFirstRender)
            {
                var value = factory();
                
                InsertAtPointer(new Frame(value));
                value.OnValueChanged += () =>
                {
                    currentEntry.Dirty = true;
                    MakeDirty();
                };
                
                return value;
            }

            if (framePointer >= frames.Count)
                throw new InvalidOperationException("Missing state variable");

            var frame = GetAtPointer();
                
            if (frame.Type != Frame.FrameType.Field)
                throw new InvalidOperationException("Invalid layout");

            return frame.Field as T;
        }

        /// <summary>
        /// Remembers given value in current composition state.
        /// </summary>
        /// <remarks>Argument is ignored when value can be found in composition state.</remarks>
        /// <param name="value">initial value</param>
        /// <typeparam name="T">type of remembered value</typeparam>
        /// <returns>Current state of the value</returns>
        /// <exception cref="InvalidOperationException">Thrown when different invocation order detected</exception>
        [PublicAPI]
        public MutableValue<T> Remember<T>(T value) => Use(isFirstRender ? new MutableValue<T>(value) : null);

        /// <summary>
        /// Remembers given value in current composition state.
        /// </summary>
        /// <remarks>Factory is only used when value cannot be found in composition state.</remarks>
        /// <param name="factory">factory used to create initial value</param>
        /// <typeparam name="T">type of remembered value</typeparam>
        /// <returns>Current state of the value</returns>
        /// <exception cref="InvalidOperationException">Thrown when different invocation order detected</exception>
        [PublicAPI]
        public MutableValue<T> RememberF<T>([NotNull] Func<T> factory) => Use(() => new MutableValue<T>(factory()));

        [PublicAPI]
        public ValueReference<T> RememberRef<T>(T value) => Use(isFirstRender ? new ValueReference<T>(value) : null);

        [PublicAPI]
        public ValueReference<T> RememberRefF<T>([NotNull] Func<T> factory) => Use(() => new ValueReference<T>(factory()));

        [PublicAPI]
        public void OnInit(Func<Action> onInit)
        {
            if (!isFirstRender)
            {
                AdvancePointer();
                return;
            }
            
            InsertAtPointer(new Frame(onInit));
        }

        [PublicAPI]
        public void OnInit(Action onInit) => OnInit(() =>
        {
            onInit();
            return null;
        });

        [PublicAPI]
        public void OnDestroy(Action onDestroy) => OnInit(() => onDestroy);

        /// <summary>
        /// Updates all outdated compositions and re-renders them.
        /// </summary>
        [PublicAPI]
        public void Update()
        {
            framePointer = 0;
            for (int i = 0; i < frames.Count; ++i)
            {
                var frame = frames.Get(i);
                
                if (frame.Type != Frame.FrameType.Entry)
                    continue;
                
                var entry = frame.Entry;

                while (entryStack.TryPeek(out var lastEntry) && lastEntry.NestingLevel >= entry.NestingLevel)
                    entryStack.Pop();

                if (!entry.Dirty)
                {
                    entryStack.Push(entry);
                    continue;
                }

                framePointer = i;
                nextEntryPreventOverride = true;
                SetNextEntryId(entry.Id);
                entry.Composition.Recompose(this);
                nextEntryPreventOverride = false;
                i = framePointer;

                var oldRender = entry.PreviouslyRendered;
                
                var newRender = entry.Composition.Render();
                
                SwapVisualElements(oldRender, newRender);
                
                entry.PreviouslyRendered = newRender;
                
                entryStack.Push(entry);
            }
            entryStack.Clear();
        }
        
        public void Dispose()
        {
            while (frames.Count > 0)
            {
                var frame = frames.Get(0);
                frame.Dispose();
                frames.RemoveAt(0);
            }
            UnregisterInstance(this);
        }
        
#if UNITY_EDITOR // for reference listing
        public IEnumerable<CompositionNode> InspectHierarchy()
        {
            var ret = new List<CompositionNode>();

            var nodeStack = new Stack<TmpNode>();
            var localEntryStack = new Stack<Frame.FrameEntry>();
            
            for (int i = 0; i < frames.Count; ++i)
            {
                var frame = frames.Get(i);

                switch (frame.Type)
                {
                    case Frame.FrameType.Entry:
                        {
                            var entry = frame.Entry;

                            while (localEntryStack.TryPeek(out var lastEntry)
                                   && (lastEntry?.NestingLevel ?? -1) >= entry.NestingLevel)
                            {
                                localEntryStack.Pop();
                                var lastNode = nodeStack.Pop();

                                if (nodeStack.TryPeek(out var parentNode))
                                    parentNode.AddChild(lastNode);
                                else
                                    ret.Add(lastNode.GetNode());
                            }

                            localEntryStack.Push(entry);
                            nodeStack.Push(new TmpNode(entry.Composition.GetType().Name));
                            
                            break;
                        }
                    case Frame.FrameType.Field:
                        {
                            if (!nodeStack.TryPeek(out var lastNode))
                                continue;

                            lastNode.AddValue(frame.Field);
                            
                            break;
                        }
                    case Frame.FrameType.Empty:
                    case Frame.FrameType.Callback:
                    default:
                        continue;
                }
            }

            while (nodeStack.Count > 0)
            {
                var lastNode = nodeStack.Pop();
                if (nodeStack.TryPeek(out var parentNode))
                    parentNode.AddChild(lastNode);
                else
                    ret.Add(lastNode.GetNode());
            }
            
            return ret;
        }
#endif
        
        private static void RegisterInstance(CompositionContext ctx)
        {
#if UNITY_EDITOR // for reference listing
            instances.Add(new(ctx));
            OnInstanceListChanged?.Invoke();
#endif
        }
        
        private static void UnregisterInstance(CompositionContext ctx)
        {
#if UNITY_EDITOR // for reference listing
            bool modified = false;
            
            instances.RemoveWhere(r =>
            {
                bool ret = !r.TryGetTarget(out var instance) || instance == ctx;

                if (ret)
                    modified = true;
                
                return ret;
            });
            if (modified)
                syncQueue.Enqueue(() => OnInstanceListChanged?.Invoke());
#endif
        }

        private static void SwapVisualElements(VisualElement oldElement, [NotNull] VisualElement newElement)
        {
            if (oldElement == null)
                return;
            
            if (oldElement == newElement)
                return;

            var parent = oldElement.parent;
            
            if (parent == null)
                return;

            int index = parent.IndexOf(oldElement);
            parent.RemoveAt(index);
            parent.Insert(index, newElement);
        }

        private void MakeDirty()
        {
            if (entryStack.Count > 0)
                throw new InvalidOperationException("Updating data before full layout render not supported");
            
            if (batchScopeLevel > 0)
            {
                dirty = true;
            }
            else
            {
                dirty = true;
                Update();
                dirty = false;
            }
        }

        private bool SetupFrame(int entryId, int currentNestingLevel, bool preventOverride, RemapHelper<int> reorderData)
        {
            if (framePointer >= frames.Count)
                return true;
            
            var frame = frames[framePointer];

            // non entry frame found at the start of current frame
            if (frame.Type != Frame.FrameType.Entry)
                throw new InvalidOperationException("Composition layout unexpected change");

            var entry = frame.Entry;
            
            // we found less nested entry frame, so we need to insert new entry here
            if (entry.NestingLevel > currentNestingLevel)
                return true;

            if (reorderData != null)
            {
                // previous element was inserted, adjust offset
                if (reorderData.LeapStart >= 0)
                {
                    reorderData.Offset += framePointer - reorderData.LeapStart;
                    reorderData.LeapStart = -1;
                }
                
                // we found some more deeply nested data than expected
                if (entry.NestingLevel != currentNestingLevel)
                    throw new InvalidOperationException(
                        "Id of composition and/or layout unexpected change");
                
                // we found matching element
                if (entry.Id == entryId)
                {
                    reorderData.RemoveFirst();
                    return false;
                }
                // first, try to find given entry and bring it closer
                (int foundStart, int foundSize) = reorderData.FindAndRemove(entryId);

                // we still don't have a match, insert new
                if (foundSize == 0)
                {
                    reorderData.LeapStart = framePointer;
                    return true;
                }
                
                var buffer = new Frame[foundSize];

                // we found a match, move found fragment into buffer
                for (int i = 0; i < foundSize; ++i)
                {
                    var tmp = frames.Get(foundStart);
                    frames.RemoveAt(foundStart);
                    buffer[i] = tmp;
                }

                int currentFramePointer = framePointer;
                
                // insert it back at the cursor
                for (int i = 0; i < foundSize; ++i)
                    InsertAtPointer(buffer[i]);

                framePointer = currentFramePointer;
                
                return false;
            }

            if (!preventOverride)
            {
                if (entryId == 0 || entry.Id != entryId || entry.NestingLevel != currentNestingLevel)
                {
                    ClearAllNested(currentNestingLevel);
                    return true;
                }
            }

            // found entry with different id, erase it
            if (entry.Id != entryId)
            {
                ClearAllNested(currentNestingLevel);
                return true;
            }

            // we found some more deeply nested data than expected
            if (entry.NestingLevel > currentNestingLevel)
                throw new InvalidOperationException(
                    "Id of composition and/or layout unexpected change");
            
            return entry.NestingLevel < currentNestingLevel;
        }
        
        private void ClearAllNested(int currentNestingLevel, bool includingCurrent = true)
        {
            if (framePointer >= frames.Count)
                return;

            var frame = frames[framePointer];
            var entry = frame.Entry;

            while (frame.Type != Frame.FrameType.Entry || entry.NestingLevel > currentNestingLevel || (includingCurrent && entry.NestingLevel == currentNestingLevel))
            {
                frame.Dispose();
                
                frames.RemoveAt(framePointer);
                if (framePointer >= frames.Count)
                    return;

                frame = frames[framePointer];
                entry = frame.Entry;
            }
        }

        private void InsertAtPointer(Frame frame) => frames.Insert(framePointer++, frame);

        private Frame GetAtPointer() => frames.Get(framePointer++);

        private void AdvancePointer() => ++framePointer;
    }
}