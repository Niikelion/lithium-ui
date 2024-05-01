using System;
using System.Reflection;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace UI.Li
{
    /// <summary>
    /// Class designed for providing state for your components.
    /// </summary>
    [PublicAPI] public sealed class Component: IComponent
    {
        public delegate IComponent StatefulComponent(ComponentState state);
        
        public event Action<VisualElement> OnRender;
        
        [NotNull] private readonly StatefulComponent composer;
        private readonly bool isStatic;
        private IComponent innerComponent;

        /// <summary>
        /// Creates Component using given function.
        /// </summary>
        /// <param name="composer">composer function used to create component.</param>
        /// <param name="isStatic">indicates whether or not structure of given component will change over time(some data or parameters can change, just not component structure).</param>
        /// <remarks><see cref="isStatic"/>only constraints returned element and not its children. For example, if you declare your component static and on first render you returned <see cref="Common.Text"/>, you must return <see cref="Common.Text"/> every time.</remarks>
        [PublicAPI] public Component([NotNull] StatefulComponent composer, bool isStatic = false)
        {
            this.composer = composer;
            this.isStatic = isStatic;
        }

        public VisualElement Render()
        {
            var ret = innerComponent?.Render() ?? throw new InvalidOperationException("Rendering component before applying context");

            OnRender?.Invoke(ret);
            
            return ret;
        }

        public void Recompose(CompositionContext context)
        {
            context.StartFrame(this);
            innerComponent = composer(new ComponentState(context));
            if (isStatic)
                context.PreventNextEntryOverride();
            innerComponent.Recompose(context);
            context.EndFrame();
        }

        public void Dispose()
        {
            innerComponent?.Dispose();
            OnRender = null;
        }

        public override string ToString() => "Component";

        public bool StateLayoutEquals(IComponent other) =>
            other is Component component &&
            isStatic == component.isStatic &&
            composer.GetMethodInfo() == component.composer.GetMethodInfo();
    }
}