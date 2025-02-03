using System;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace UI.Li.Utils
{
    /// <summary>
    /// Lightweight static component class that uses provided function to render component.
    /// </summary>
    /// <remarks>Can be used to easily wrap VisualElements</remarks>
    [PublicAPI] public sealed class LightComponent: IComponent
    {
        private event Action<VisualElement> OnRender;
        
        event Action<VisualElement> IComponent.OnRender
        {
            add => OnRender += value;
            remove => OnRender -= value;
        }

        public IComponent UnderlyingComponent => this;

        [NotNull] private readonly Func<VisualElement, VisualElement> renderer;
        private VisualElement previouslyRendered;

        /// <summary>
        /// Creates component using renderer function.
        /// </summary>
        [PublicAPI] public LightComponent([NotNull] Func<VisualElement, VisualElement> renderer) => this.renderer = renderer;

        /// <summary>
        /// Creates component using renderer function.
        /// </summary>
        [PublicAPI] public LightComponent([NotNull] Func<VisualElement> renderer) : this(_ => renderer()) { }

        public VisualElement Render()
        {
            var ret = renderer(previouslyRendered);
            
            OnRender?.Invoke(ret);
            
            return ret;
        }

        public void Recompose(CompositionContext context)
        {
            previouslyRendered = context.StartFrame(this);
            context.EndFrame();
        }

        public bool StateLayoutEquals(IComponent other) => other is LightComponent;

        public void Dispose()
        {
            previouslyRendered = null;
            OnRender = null;
        }

        public override string ToString() => "LightComponent";
    }
}