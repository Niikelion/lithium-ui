using System;
using JetBrains.Annotations;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UI.Li.Common
{
    /// <summary>
    /// Component representing <see cref="VisualElement"/> without children.
    /// </summary>
    /// <remarks>Good base class for compositions that supports styling and callbacks of <see cref="VisualElement"/></remarks>
    [PublicAPI] public class Element: IComponent
    {
        public IComponent UnderlyingComponent => this;

        private event Action<VisualElement> OnRender;

        event Action<VisualElement> IComponent.OnRender
        {
            add => OnRender += value;
            remove => OnRender -= value;
        }

        /// <summary>
        /// Reference to previously rendered element.
        /// </summary>
        /// <remarks>It is not guaranteed that this <see cref="VisualElement"/> will have expected type(for example <see cref="UnityEngine.UIElements.Label"/>) so it should be runtime checked in every function using this field.</remarks>
        /// <seealso cref="Use{T}(VisualElement, bool)"/>
        protected VisualElement PreviouslyRendered { get; private set; }

        /// <summary>
        /// Recomposition strategy used by element.
        /// </summary>
        /// <seealso cref="CompositionContext.RecompositionStrategy"/>
        protected virtual CompositionContext.RecompositionStrategy RecompositionStrategy =>
            CompositionContext.RecompositionStrategy.Override;
        
        private readonly IManipulator[] manipulators;

        /// <summary>
        /// Constructs <see cref="Element"/> instance.
        /// </summary>
        /// <param name="manipulators">manipulators <seealso cref="IManipulator"/></param>
        /// <returns></returns>
        [NotNull] public static Element V(params IManipulator[] manipulators) => new(manipulators: manipulators);
        [NotNull] public static Element V(IEnumerable<IManipulator> manipulators) => new (manipulators: manipulators);

        public Element(IEnumerable<IManipulator> manipulators = null)
        {
            this.manipulators = manipulators?.ToArray() ?? Array.Empty<IManipulator>();
        }
        
        public VisualElement Render()
        {
            var coreElement = GetElement(PreviouslyRendered);
            var ret = PrepareElement(coreElement);
            
            OnRender?.Invoke(ret);
            PreviouslyRendered = ret;
            return ret;
        }

        public void Recompose(CompositionContext context)
        {
            PreviouslyRendered = context.StartFrame(this, RecompositionStrategy);
            
            OnState(context);
            
            context.EndFrame();
        }

        public virtual void Dispose()
        {
            if (PreviouslyRendered != null)
                CompositionContext.ElementUserData.CleanUp(PreviouslyRendered);
            PreviouslyRendered = null;
        }

        public virtual bool StateLayoutEquals(IComponent other) => GetType() == other.GetType();

        /// <summary>
        /// Used to obtain <see cref="VisualElement"/> instance. Override this method if you need instance of <see cref="VisualElement"/> subclass.
        /// </summary>
        /// <seealso cref="Use{T}(VisualElement, bool)"/>
        /// <remarks>During render, <see cref="PreviouslyRendered"/> is passed to this function and return value is passed to <see cref="PrepareElement"/> to obtain render result. When overriding you don't need to call base implementation.</remarks>
        /// <param name="source">cached element</param>
        /// <returns></returns>
        [NotNull] protected virtual VisualElement GetElement([CanBeNull] VisualElement source) => Use<VisualElement>(source, true);

        /// <summary>
        /// Called every component. Override it if you need to store anything in the state.
        /// </summary>
        /// <remarks>Works similar to <see cref="IComponent.Recompose"/>, but you don't need to call <see cref="CompositionContext.StartFrame"/> and <see cref="CompositionContext.EndFrame"/>. When overriding you don't need to call base implementation.</remarks>
        /// <param name="context">component context</param>
        protected virtual void OnState(CompositionContext context) { }

        /// <summary>
        /// Postprocess method used to apply styles and events.
        /// </summary>
        /// <remarks>If you need to override some styles override this method and modify styles of returned element from base implementation.</remarks>
        /// <param name="target"><see cref="VisualElement"/> to apply styles and callbacks to</param>
        /// <returns>Returns element passed as <paramref name="target"/>.</returns>
        [NotNull] protected virtual VisualElement PrepareElement([NotNull] VisualElement target)
        {
            CompositionContext.ElementUserData.AddManipulators(target, manipulators);
            return target;
        }

        /// <summary>
        /// Returns given element if it is an instance of the given type, or new instance otherwise.
        /// </summary>
        /// <param name="source">some element, can be null</param>
        /// <param name="clear">if true, clears all children if element is reused</param>
        /// <typeparam name="T">expected type</typeparam>
        /// <returns></returns>
        [NotNull] protected T Use<T>([CanBeNull] VisualElement source, bool clear = false) where T : VisualElement, new()
        {
            if (source is not T element || element.GetType() != typeof(T)) return new ();

            if (clear)
                element.contentContainer.Clear();

            return element;
        }

        [NotNull] protected T Use<T>([CanBeNull] VisualElement source, [NotNull] Func<T> factory, [NotNull] Func<T, bool> filter, bool clear = false) where T : VisualElement
        {
            if (source is not T element || element.GetType() != typeof(T)) return factory();

            if (!filter(element))
                return factory();
            
            if (clear)
                element.Clear();

            return element;
        }

        protected void AddCleanup(VisualElement element, Action onCleanup) => CompositionContext.ElementUserData.AppendCleanupAction(element, onCleanup);

        public override string ToString() => GetType().Name;
    }

    [PublicAPI]
    public class Element<T>: Element where T : VisualElement, new()
    {
        protected sealed override VisualElement GetElement(VisualElement source) => Use<T>(source);

        protected sealed override VisualElement PrepareElement(VisualElement target)
        {
            // cast is fine, since we have full control over type returned by GetElement
            var ret = base.PrepareElement(target) as T;
            Debug.Assert(ret != null);
            return PrepareElement(ret);
        }

        [NotNull] protected virtual T PrepareElement([NotNull] T target) => target;

        public Element(IEnumerable<IManipulator> manipulators = null): base(manipulators) { }
    }
}