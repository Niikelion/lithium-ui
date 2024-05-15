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
        public delegate IComponent OldStatefulComponent(ComponentState state);
        public delegate IComponent StatefulComponent();
        
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
        [Obsolete("Use variant with argument-less composer")] public Component([NotNull] OldStatefulComponent composer, bool isStatic = false)
        {
            this.composer = () => composer(new());
            this.isStatic = isStatic;
        }
        
        /// <summary>
        /// Creates Component using given function.
        /// </summary>
        /// <param name="composer">composer function used to create component.</param>
        /// <param name="isStatic">indicates whether or not structure of given component will change over time(some data or parameters can change, just not component structure).</param>
        /// <remarks><see cref="isStatic"/>only constraints returned element and not its children. For example, if you declare your component static and on first render you returned <see cref="Common.Text"/>, you must return <see cref="Common.Text"/> every time.</remarks>
        public Component([NotNull] StatefulComponent composer, bool isStatic = false)
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
            ComponentState.ProvideInternalContext(context);
            context.StartFrame(this);
            innerComponent = composer();
            if (isStatic)
                context.PreventNextEntryOverride();
            innerComponent.Recompose(context);
            context.EndFrame();
            ComponentState.ProvideInternalContext(null);
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