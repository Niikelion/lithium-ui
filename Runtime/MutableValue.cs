using System;
using JetBrains.Annotations;

namespace UI.Li
{
    /// <summary>
    /// Interface representing mutable state variable.
    /// </summary>
    [PublicAPI] public interface IMutableValue: IDisposable
    {
        /// <summary>
        /// Event called every time value changes.
        /// </summary>
        [PublicAPI] event Action OnValueChanged;
    }
    
    /// <summary>
    /// Simple implementation that notifies observers every time <see cref="Value"/> is reassigned.
    /// </summary>
    /// <typeparam name="T">type of stored value</typeparam>
    public class MutableValue<T>: IMutableValue
    {
        public event Action OnValueChanged;

        /// <summary>
        /// Stored value.
        /// </summary>
        /// <remarks>Calling setter invokes <see cref="OnValueChanged"/>.</remarks>
        [PublicAPI]
        public T Value
        {
            get => value;
            set
            {
                this.value = value;
                OnValueChanged?.Invoke();
            }
        }
        
        private T value;

        /// <summary>
        /// Constructs <see cref="MutableValue{T}"/> using given <see cref="value"/>.
        /// </summary>
        /// <param name="value">initial value</param>
        [PublicAPI]
        public MutableValue(T value)
        {
            this.value = value;
        }
        
        public static implicit operator T(MutableValue<T> v) => v.Value;
        
        public void Dispose() => OnValueChanged = null;
    }

    public class ValueReference<T> : IMutableValue
    {
        public event Action OnValueChanged;

        [PublicAPI]
        public T Value;

        [PublicAPI]
        public ValueReference(T value) => Value = value;

        [PublicAPI]
        public void NotifyChanged() => OnValueChanged?.Invoke();
        
        public void Dispose() => OnValueChanged = null;
    }
}