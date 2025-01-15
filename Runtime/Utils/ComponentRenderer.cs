using System;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace UI.Li.Utils
{
    /// <summary>
    /// Utility class used to render components.
    /// </summary>
    /// <remarks>Designed to be used from outside Lithium system to render components.</remarks>
    [PublicAPI] public class ComponentRenderer: IDisposable
    {
        private readonly CompositionContext context;
        private readonly IComponent component;

        /// <summary>
        /// Constructs renderer using given component.
        /// </summary>
        /// <param name="component">component to render</param>
        /// <param name="name">name of context to be displayed in debugger</param>
        /// <param name="hidden">if true, context will not be visible on the instance list</param>
        public ComponentRenderer([NotNull] IComponent component, string name = "Unnamed", bool hidden = false): this(() => component, name, hidden) {}

        public ComponentRenderer([NotNull] Func<IComponent> componentFactory, string name = "Unnamed",
            bool hidden = false)
        {
            context = new (name, hidden);
            using var _ = context.ProvideCompositionContext();
            
            component = componentFactory();
        }
        
        /// <summary>
        /// Recomposes component, <see cref="IComponent.Recompose"/>.
        /// </summary>
        public void Update()
        {
            using var _ = context.ProvideCompositionContext();
            component.Recompose(context);
        }

        /// <summary>
        /// Renders component, <see cref="IComponent.Render"/>.
        /// </summary>
        /// <returns></returns>
        public VisualElement Render() => component.Render();

        /// <summary>
        /// Updates and renders component at the same time.
        /// </summary>
        /// <returns></returns>
        public VisualElement UpdateAndRender()
        {
            Update();
            return Render();
        }
        
        /// <summary>
        /// Disposes of the managed component and its state.
        /// </summary>
        /// <remarks>Disposed renderer shouldn't be used.</remarks>
        public void Dispose()
        {
            context.Dispose();
            component.Dispose();
        }

        /// <summary>
        /// See <see cref="CompositionContext.MakeHidden(bool)"/>
        /// </summary>
        public void MakeHidden(bool hidden = true) => context.MakeHidden(hidden);
    }
}