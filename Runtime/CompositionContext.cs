using System;
using System.Linq;
using UnityEngine;
using UI.Li.Internal;
using JetBrains.Annotations;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using Unity.Profiling;

namespace UI.Li
{
    [PublicAPI] public class CompositionContext: IDisposable
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
            /// Override every non-matching or unnamed entry.
            /// </summary>
            Override,
            /// <summary>
            /// Try to reorder previous sub-compositions to match current layout. Introduces overhead, use <see cref="CompositionContext.RecompositionStrategy.Override"/> when possible.
            /// </summary>
            Reorder
        }

        [PublicAPI] public class ElementUserData
        {
            public static VisualElement AppendCleanupAction(VisualElement element, Action onCleanup)
            {
                if (element.userData is not ElementUserData data)
                {
                    if (element.userData != null)
                        Debug.LogError($"Overriding custom user data on {element.GetType().Name} element. You may experience unexpected behaviour due to the data loss.");

                    data = new ();
                    element.userData = data;
                }

                data.onCleanup = onCleanup + data.onCleanup;

                return element;
            }

            public static VisualElement AddManipulator(VisualElement element, IManipulator manipulator)
            {
                if (element.userData is not ElementUserData data)
                {
                    if (element.userData != null)
                        Debug.LogError($"Overriding custom user data on {element.GetType().Name} element. You may experience unexpected behaviour due to the data loss.");

                    data = new();
                    element.userData = data;
                }

                data.manipulators.Add(manipulator);
                element.AddManipulator(manipulator);
                
                return element;
            }

            public static VisualElement AddManipulators(VisualElement element, IEnumerable<IManipulator> manipulators)
            {
                foreach (var manipulator in manipulators)
                    AddManipulator(element, manipulator);
                
                return element;
            }
            
            public static VisualElement CleanUp(VisualElement element)
            {
                if (element == null)
                    return null;
                
                if (element.userData is not ElementUserData data)
                    return element;

                foreach (var manipulator in data.manipulators)
                    element.RemoveManipulator(manipulator);
                
                data.manipulators.Clear();
                
                data.onCleanup?.Invoke();
                data.onCleanup = null;

                return element;
            }

            private Action onCleanup;
            private readonly HashSet<IManipulator> manipulators;

            private ElementUserData()
            {
                manipulators = new ();
            }
        }

        [NotNull] public delegate T FactoryDelegate<out T>();

        internal readonly struct Frame: IDisposable
        {
            public class FrameEntry: IDisposable
            {
                public ReadOnlyDictionary<Type, object> LocalContexts => new(addedContexts ?? new Dictionary<Type, object>());
                
                public readonly int Id;
                public readonly int NestingLevel;
                [NotNull] public IComponent Component;
                public bool Dirty;
                public bool Crashed;
                public RemapHelper<int> Reordering;
                public VisualElement PreviouslyRendered;
                private Dictionary<Type, object> overriddenContexts;
                private Dictionary<Type, object> addedContexts;
                
                public FrameEntry(int id, int nestingLevel, [NotNull] IComponent component)
                {
                    Id = id;
                    NestingLevel = nestingLevel;
                    Component = component;
                    Dirty = true;
                    PreviouslyRendered = null;
                    Crashed = false;
                }

                public void PushContext(Dictionary<Type, object> contexts, object context)
                {
                    if (context == null)
                        return;

                    var type = context.GetType();

                    overriddenContexts ??= new();
                    addedContexts ??= new();

                    if (overriddenContexts.ContainsKey(type))
                    {
                        addedContexts[type] = context;
                        contexts[type] = context;
                        return;
                    }

                    if (contexts.TryGetValue(type, out object oldContext))
                        contexts[type] = context;
                    else
                        contexts.Add(type, context);

                    overriddenContexts.Add(type, oldContext);
                }

                public void ApplyContexts(Dictionary<Type, object> contexts)
                {
                    if (addedContexts == null)
                        return;

                    foreach (var contextEntry in addedContexts)
                        contexts[contextEntry.Key] = contextEntry.Value;
                }

                public void RevertContexts(Dictionary<Type, object> contexts)
                {
                    if (overriddenContexts == null)
                        return;

                    foreach (var contextEntry in overriddenContexts)
                    {
                        if (contextEntry.Value == null)
                            contexts.Remove(contextEntry.Key);
                        else
                            contexts[contextEntry.Key] = contextEntry.Value;
                    }
                }

                public void CleanupReordering() => Reordering = null;

                public void Dispose()
                {
                    Component.Dispose();
                    CleanupReordering();
                }

                public void Crash() => Crashed = true;
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
            private readonly Action disposeCallback;
            
            public Frame([NotNull] IComponent component, int nestingLevel, int entryId)
            {
                Type = FrameType.Entry;
                Field = null;
                Entry = new (entryId, nestingLevel, component);
                disposeCallback = null;
            }

            public Frame(IMutableValue field)
            {
                Type = FrameType.Field;
                Field = field;
                Entry = null;
                disposeCallback = null;
            }

            public Frame(Func<Action> onInit)
            {
                Type = FrameType.Callback;
                Field = null;
                Entry = null;
                disposeCallback = onInit();
            }

            public void Dispose()
            {
                Entry?.Component.Dispose();
                Field?.Dispose();
                disposeCallback?.Invoke();
            }
        }
        
        [PublicAPI] public class InspectedNode
        {
            public readonly ReadOnlyCollection<IMutableValue> Values;
            public readonly ReadOnlyCollection<InspectedNode> Children;

            public string Name => entry.Component.ToString();
            public IComponent Component => entry.Component;
            public int Id => entry.Id;
            public bool Crashed => entry.Crashed;
            public VisualElement RenderedElement => entry.PreviouslyRendered;
            public ReadOnlyDictionary<Type, object> Contexts => entry.LocalContexts;
            
            private readonly Frame.FrameEntry entry;
            private readonly WeakReference<CompositionContext> ctx;

            public void Recompose()
            {
                if (!ctx.TryGetTarget(out var context))
                    return;
                entry.Dirty = true;
                context.MakeDirty();
            }
            
            internal InspectedNode(Frame.FrameEntry entry, CompositionContext ctx, List<IMutableValue> values, List<InspectedNode> children)
            {
                this.entry = entry;
                this.ctx = new (ctx);
                Values = values.AsReadOnly();
                Children = children.AsReadOnly();
            }
        }
        
        private class TemporaryInspectionNode
        {
            private readonly Frame.FrameEntry entry;
            private readonly List<IMutableValue> values = new();
            private readonly List<TemporaryInspectionNode> children = new();

            public TemporaryInspectionNode(Frame.FrameEntry entry) => this.entry = entry;

            public void AddValue(IMutableValue value) => values.Add(value);
            public void AddChild(TemporaryInspectionNode child) => children.Add(child);

            public InspectedNode GetNode(CompositionContext ctx) => new (entry, ctx, values, children.Select(n => n.GetNode(ctx)).ToList());
        }
        
        [NotNull] public static IEnumerable<CompositionContext> Instances => instances.Select(r => r.TryGetTarget(out var instance) ? instance : null).Where(i => i != null);
        public static event Action OnInstanceListChanged;
        private static readonly ConcurrentQueue<Action> syncQueue = new();

        private static readonly ProfilerMarker updateProfileMarker = new ("Lithium.CompositionContext.Update");
        
        [NotNull] [PublicAPI] public readonly string Name;

        public event Action OnUpdate;
        
        private static readonly HashSet<WeakReference<CompositionContext>> instances = new(new WeakReferenceTargetComparer<CompositionContext>());

        private readonly GapBuffer<Frame> frames = new();
        private int framePointer;
        private bool isFirstRender;

        private readonly Stack<Frame.FrameEntry> entryStack = new();
        private readonly Dictionary<Type, object> contexts = new();

        private bool dirty;
        
        private int batchScopeLevel;
        private int nextEntryId;
        private bool nextEntryPreventOverride;
        private IComponent nextEntryOrigin;

        static CompositionContext()
        {
            UnityEditor.EditorApplication.update += () =>
            {
                while (syncQueue.TryDequeue(out var action))
                    action();
            };
        }
        
        ~CompositionContext() => Dispose();

        public CompositionContext(string name = "Unnamed", bool hidden = false)
        {
            Name = name;
            if (!hidden)
                RegisterInstance(this);
        }

        /// <summary>
        /// Hides or shows context in the global context registry.
        /// </summary>
        public void MakeHidden(bool hidden = true)
        {
            if (hidden)
                UnregisterInstance(this);
            else
                RegisterInstance(this);
        }
        
        /// <summary>
        /// Enters update batching scope. <seealso cref="BatchOperations"/>
        /// </summary>
        public void EnterBatchScope() => ++batchScopeLevel;

        /// <summary>
        /// Leaves update batching scope. <seealso cref="BatchOperations"/>
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when there is no scope to leave.</exception>
        public void LeaveBatchScope()
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
        public BatchScope BatchOperations() => new (this);
        
        /// <summary>
        /// Sets component entry id for next recomposition. 
        /// </summary>
        /// <remarks>Difficult to use outside custom component implementation. To give id to component use <see cref="Li.Utils.Utils.Id"/></remarks>
        /// <param name="entryId">ID for next component</param>
        public void SetNextEntryId(int entryId = 0) => nextEntryId = entryId;

        public void SetNextEntryOrigin(IComponent origin) => nextEntryOrigin ??= origin;
        
        /// <summary>
        /// Forces preferring updates to overrides in the next recomposition. 
        /// </summary>
        /// <remarks>Should not be used outside custom component implementation.</remarks>
        public void PreventNextEntryOverride() => nextEntryPreventOverride = true;
        
        /// <summary>
        /// Starts composing given component.
        /// </summary>
        /// <remarks>Should be called at the beginning of <see cref="IComponent.Recompose"/> in custom component implementation.</remarks>
        /// <returns>Previously rendered element for given component.</returns>
        public VisualElement StartFrame([NotNull] IComponent component, RecompositionStrategy strategy = RecompositionStrategy.Override)
        {
            SetNextEntryOrigin(component);
            int currentNestingLevel = entryStack.TryPeek(out var lastEntry) ? lastEntry.NestingLevel + 1 : 0;

            int currentEntryId = nextEntryId;
            component = TakeOrigin();
            
            isFirstRender = SetupFrame(currentEntryId, currentNestingLevel, nextEntryPreventOverride, lastEntry?.Reordering, component);
            nextEntryPreventOverride = false;
            nextEntryId = 0;

            VisualElement previouslyRendered = null;

            Frame.FrameEntry currentEntry;
            
            if (isFirstRender)
            {
                InsertAtPointer(new (
                    entryId: currentEntryId,
                    nestingLevel: currentNestingLevel,
                    component: component
                ));
                
                currentEntry = frames[framePointer - 1].Entry;
                
                component.OnRender += v => currentEntry.PreviouslyRendered = v;
            }
            else
            {
                var frame = GetAtPointer();

                currentEntry = frame.Entry;

                if (!Equals(currentEntry.Component, component))
                {
                    currentEntry.Component.Dispose();
                    currentEntry.Component = component;
                    component.OnRender += v => currentEntry.PreviouslyRendered = v;
                }
                previouslyRendered = frame.Entry.PreviouslyRendered;
            }
            
            PushFrameEntry(currentEntry);
            currentEntry.Dirty = false;
            currentEntry.Crashed = false;

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
        /// Ends composing current component.
        /// </summary>
        /// <remarks>Should be called at the end of <see cref="IComponent.Recompose"/> in custom component implementation.</remarks>
        public void EndFrame()
        {
            if (entryStack.Count == 0)
                throw new InvalidOperationException("EndFrame called more times than StartFrame");
            
            int currentNestingLevel = entryStack.TryPeek(out var currentEntry) ? currentEntry.NestingLevel : 0;
            
            ClearAllNested(currentNestingLevel, false);

            PopFrameEntry().CleanupReordering();
        }

        /// <summary>
        /// Adds given state value to current component state.
        /// </summary>
        /// <seealso cref="UI.Li.ComponentStateExtensions.RememberF{T}"/>
        /// <param name="factory">factory of state value to add</param>
        /// <typeparam name="T">type of state value, must implement <see cref="IMutableValue"/></typeparam>
        /// <returns>Returns current state of the value</returns>
        /// <exception cref="InvalidOperationException">Thrown when different invocation order detected</exception>
        [NotNull]
        public T Use<T>([NotNull] FactoryDelegate<T> factory) where T : class, IMutableValue
        {
            // there is no parent to host this variable/hook
            if (!entryStack.TryPeek(out var currentEntry))
                throw new InvalidOperationException("Using state outside composable function");

            if (isFirstRender)
            {
                var value = factory();
                
                InsertAtPointer(new (value));
                value.OnValueChanged += () =>
                {
                    currentEntry.Dirty = true;
                    MakeDirty();
                };
                
                return value;
            }

            if (framePointer >= frames.Count)
                throw new InvalidOperationException("Reached end of state. Make sure hook calls are consistent across renders.");

            var frame = GetAtPointer();
                
            if (frame.Type != Frame.FrameType.Field)
                throw new InvalidOperationException("Reached end of state. Make sure hook calls are consistent across renders.");

            return frame.Field as T ?? throw new InvalidOperationException($"State variable changed its type from {frame.Field?.GetType().FullName} to {typeof(T).FullName}. Make sure hook calls are consistent across renders.");
        }

        public void ProvideContext<T>(T context)
        {
            if (!entryStack.TryPeek(out var currentEntry))
                throw new InvalidOperationException("Using context outside composable function");

            currentEntry.PushContext(contexts, context);
        }

        public T UseContext<T>()
        {
            var type = typeof(T);

            if (!contexts.TryGetValue(type, out var context))
            {
                Debug.LogError($"Missing context {type.FullName}");
                return default;
            }

            if (context is not T ctx)
                throw new("Internal error: context type mismatch. This should never happen.");

            return ctx;
        }

        public void OnInit(Func<Action> onInit)
        {
            if (!isFirstRender)
            {
                AdvancePointer();
                return;
            }
            
            InsertAtPointer(new (onInit));
        }

        /// <summary>
        /// Updates all outdated compositions and re-renders them.
        /// </summary>
        public void Update()
        {
            updateProfileMarker.Begin();
            
            framePointer = 0;
            for (int i = 0; i < frames.Count; ++i)
            {
                var frame = frames.Get(i);
                
                if (frame.Type != Frame.FrameType.Entry)
                    continue;
                
                var entry = frame.Entry;

                while (entryStack.TryPeek(out var lastEntry) && lastEntry.NestingLevel >= entry.NestingLevel)
                    PopFrameEntry();

                if (!entry.Dirty)
                {
                    PushFrameEntry(entry);
                    continue;
                }

                framePointer = i;
                nextEntryPreventOverride = true;
                var oldRender = ElementUserData.CleanUp(entry.PreviouslyRendered);
                
                SetNextEntryId(entry.Id);
                entry.Component.Recompose(this);
                nextEntryPreventOverride = false;
                i = framePointer - 1;
                
                var newRender = entry.Component.Render();
                
                SwapVisualElements(oldRender, newRender);
                
                entry.PreviouslyRendered = newRender;
                
                PushFrameEntry(entry);
            }
            ClearFrameStack();
            
            updateProfileMarker.End();
            OnUpdate?.Invoke();
        }
        
        /// <summary>
        /// Performs component panic.
        /// </summary>
        /// <remarks>
        /// Component panic clears all the date of the current component.
        /// You should use it when recovering from exception during recomposition to salvage rest of the hierarchy.
        /// </remarks>
        public void Panic(IComponent originComponent)
        {
            // find frame containing component entry
            var componentFrame = UnwindToComponent(originComponent);
            
            if (!componentFrame.HasValue)
                throw new("Found corrupted hierarchy during panic!");
            
            var entry = componentFrame.Value.Entry;
            
            // clear data
            ClearAllNested(entry.NestingLevel, false);
            // mark as crashed
            entry.Crash();
        }
        
        public void Dispose()
        {
            while (frames.Count > 0)
            {
                var frame = frames.Get(0);
                frame.Dispose();
                frames.RemoveAt(0);
            }

            OnUpdate = null;
            
            UnregisterInstance(this);
        }
        
        public IEnumerable<InspectedNode> InspectHierarchy()
        {
            var ret = new List<InspectedNode>();

            var nodeStack = new Stack<TemporaryInspectionNode>();
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
                                    ret.Add(lastNode.GetNode(this));
                            }

                            localEntryStack.Push(entry);
                            nodeStack.Push(new (entry));
                            
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
                    ret.Add(lastNode.GetNode(this));
            }
            
            return ret;
        }

        private void PushFrameEntry(Frame.FrameEntry entry)
        {
            entryStack.Push(entry);
            entry.ApplyContexts(contexts);
        }

        private Frame.FrameEntry PopFrameEntry()
        {
            var entry = entryStack.Pop();
            entry.RevertContexts(contexts);
            return entry;
        }

        private void ClearFrameStack()
        {
            entryStack.Clear();
            contexts.Clear();
        }

        private void MakeDirty()
        {
            if (entryStack.Count > 0)
                throw new InvalidOperationException("Updating data before full layout render is not supported.");

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

        /// <summary>
        /// Setups current stack pointer so it points to the start of the entry for provided component.
        /// It returns whether inserting new entry at pointer is needed.
        /// </summary>
        /// <param name="entryId">identifier for the entry, 0 for empty</param>
        /// <param name="currentNestingLevel">current depth of the component</param>
        /// <param name="preventOverride">flag that enabled strict checking mode and increases state prevention measures</param>
        /// <param name="reorderData">cached data needed for identifier based reordering</param>
        /// <param name="component">component that is being composed</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        private bool SetupFrame(int entryId, int currentNestingLevel, bool preventOverride,
            RemapHelper<int> reorderData, IComponent component)
        {
            if (framePointer >= frames.Count)
                return true;

            var frame = frames[framePointer];

            // non entry frame found at the start of current frame
            if (frame.Type != Frame.FrameType.Entry)
                throw new InvalidOperationException("Component layout unexpected change");

            var entry = frame.Entry;
            preventOverride = preventOverride || entry.Component.StateLayoutEquals(component);

            // start of less nested element means there is no more data of parent element, we should insert new entry
            if (entry.NestingLevel < currentNestingLevel) return true;

            // handle reordering
            if (entry.NestingLevel == currentNestingLevel && reorderData != null)
            {
                // previous element was inserted, adjust offset
                if (reorderData.LeapStart >= 0)
                {
                    reorderData.Offset += framePointer - reorderData.LeapStart;
                    reorderData.LeapStart = -1;
                }
                
                // we found matching element
                if (entry.Id == entryId)
                {
                    reorderData.RemoveFirst();
                    return false;
                }

                // first, try to find given entry and bring it closer
                var (foundStart, foundSize) = reorderData.FindAndRemove(entryId);

                // we still don't have a match, insert new;
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
            
            // we found more nested entry
            if (entry.NestingLevel > currentNestingLevel)
            {
                // throw error if we can't override it
                if (preventOverride)
                    throw new InvalidOperationException("Id of component and/or layout unexpected change");

                // otherwise we can clear it and insert new entry
                ClearAllNested(currentNestingLevel);
                return true;
            }

            // found entry with different id, erase it and insert new entry
            if (entry.Id != entryId)
            {
                ClearAllNested(currentNestingLevel);
                return true;
            }

            // matching custom id or matching lack of id but override prevention is on
            if (entryId != 0 || preventOverride) return false;
            
            ClearAllNested(currentNestingLevel);

            return true;
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

        private Frame? UnwindToComponent(IComponent component)
        {
            for (int i = framePointer-1; i >= 0; i--)
            {
                var frame = frames[i];
                if (frame.Type != Frame.FrameType.Entry)
                    continue;

                if (frame.Entry.Component == component)
                {
                    framePointer = i;
                    return frame;
                }
                
                if (!entryStack.TryPeek(out var stackEntry) || stackEntry != frame.Entry)
                    continue;

                PopFrameEntry().CleanupReordering();
            }

            return null;
        }

        private void InsertAtPointer(Frame frame) => frames.Insert(framePointer++, frame);

        private Frame GetAtPointer() => frames.Get(framePointer++);

        private void AdvancePointer() => ++framePointer;

        private IComponent TakeOrigin()
        {
            var ret = nextEntryOrigin;
            nextEntryOrigin = null;
            return ret;
        }
        
        private static void RegisterInstance(CompositionContext ctx)
        {
            if (instances.Add(new(ctx)))
                OnInstanceListChanged?.Invoke();
        }
        
        private static void UnregisterInstance(CompositionContext ctx)
        {
            if (instances.RemoveWhere(r => !r.TryGetTarget(out var instance) || instance == ctx) > 0)
                syncQueue.Enqueue(() => OnInstanceListChanged?.Invoke());
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
    }
}