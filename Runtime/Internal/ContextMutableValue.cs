using System;

namespace UI.Li.Internal
{
    public class ContextMutableValue<T>: MutableValue<T>
    {
        public override T Value
        {
            get => base.Value;
            set
            {
                if (!context.TryGetTarget(out var ctx))
                    return;

                using var guard = ctx.BatchOperations();

                base.Value = value;
            }
        }

        private readonly WeakReference<CompositionContext> context;

        public ContextMutableValue(T value, CompositionContext ctx) : base(value)
        {
            context = new WeakReference<CompositionContext>(ctx);
        }
    }
}