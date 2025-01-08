using System;
using JetBrains.Annotations;
using UnityEngine;

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
        public event Action OnValueChanged;
    }
    
    /// <summary>
    /// Simple implementation that notifies observers every time <see cref="Value"/> is reassigned.
    /// </summary>
    /// <typeparam name="T">type of stored value</typeparam>
    [PublicAPI, Serializable] public class MutableValue<T>: IMutableValue
    {
        public event Action OnValueChanged;
        
        private readonly bool propagateDispose;

        /// <summary>
        /// Stored value.
        /// </summary>
        /// <remarks>Calling setter invokes <see cref="OnValueChanged"/>.</remarks>
        public virtual T Value
        {
            get => value;
            set
            {
                this.value = value;
                NotifyChanged();
            }
        }
        
        [SerializeField] private T value;

        /// <summary>
        /// Constructs <see cref="MutableValue{T}"/> using given <see cref="value"/>.
        /// </summary>
        /// <param name="value">initial value</param>
        /// <param name="propagateDispose">enables propagating dispose call to the stored value</param>
        public MutableValue(T value, bool propagateDispose = false)
        {
            this.value = value;
            this.propagateDispose = propagateDispose;
        }
        
        public static implicit operator T(MutableValue<T> v) => v.Value;
        
        public void Dispose()
        {
            OnValueChanged = null;
            if (propagateDispose && value is IDisposable disposable)
                disposable.Dispose();
        }

        public override string ToString() => value?.ToString();
        
        protected void NotifyChanged() => OnValueChanged?.Invoke();
    }

    [PublicAPI, Serializable] public class ValueReference<T> : IMutableValue
    {
        public event Action OnValueChanged;

        public T Value;
     
        private readonly bool propagateDispose;
        
        public ValueReference(T value, bool propagateDispose = false)
        {
            Value = value;
            this.propagateDispose = propagateDispose;
        }

        public static implicit operator T(ValueReference<T> v) => v.Value;
        
        public void NotifyChanged() => OnValueChanged?.Invoke();
        
        public void Dispose()
        {
            OnValueChanged = null;
            if (propagateDispose && Value is IDisposable disposable)
                disposable.Dispose();
        }

        public override string ToString() => Value.ToString();
    }
}