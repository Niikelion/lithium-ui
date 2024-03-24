using System;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace UI.Li.Utils
{
    [PublicAPI]
    public abstract class Wrapper: IComponent
    {
        protected readonly IComponent InnerComponent;

        protected Wrapper(IComponent innerComponent) => InnerComponent = innerComponent;

        protected VisualElement PreviouslyRendered { get; private set; }

        public virtual void Dispose() => InnerComponent.Dispose();

        public event Action<VisualElement> OnRender
        {
            add => InnerComponent.OnRender += value;
            remove => InnerComponent.OnRender -= value;
        }
        public virtual VisualElement Render()
        {
            var ret = InnerComponent.Render();
            PreviouslyRendered = ret;
            return ret;
        }

        public void Recompose(CompositionContext context)
        {
            context.SetNextEntryOrigin(this);
            BeforeInnerRecompose(context);
            InnerComponent.Recompose(context);
        }

        protected virtual void BeforeInnerRecompose(CompositionContext context) { }

        public override string ToString() => InnerComponent.ToString();

        public virtual bool StateLayoutEquals(IComponent other) =>
            other is Wrapper wrapper && GetType() == wrapper.GetType() && InnerComponent.StateLayoutEquals(wrapper.InnerComponent);
    }
}