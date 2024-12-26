using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

using static UI.Li.ComponentState;

namespace UI.Li.Async
{
    [PublicAPI] public static class Async
    {
        [PublicAPI]
        public class AsyncMutable<T> : IMutableValue
        {
            public event Action OnValueChanged;

            public bool HasError => task.IsFaulted;
            public bool HasResult => task.IsCompletedSuccessfully;

            public T Result => HasResult ? task.Result : throw new("Value not ready!");
            public Exception Error => HasError ? task.Exception : null;
            
            [NotNull] private readonly Task<T> task;
            
            public AsyncMutable([NotNull] Task<T> task)
            {
                this.task = task;
                task.ContinueWith(_ => OnValueChanged?.Invoke());
            }
            
            public void Dispose() => task.Dispose();
        }

        public static AsyncMutable<T> Load<T>(CompositionContext.FactoryDelegate<Task<T>> loader) =>
            Use(() => new AsyncMutable<T>(loader()));
    }
}