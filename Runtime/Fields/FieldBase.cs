using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UI.Li.Common;
using UnityEngine.UIElements;

namespace UI.Li.Fields
{
    public abstract class FieldBase<TValue, TElem>: Element<TElem> where TElem : BaseField<TValue>, new()
    {
        [NotNull] private readonly Action<TValue> onValueChanged;
        private readonly TValue initialValue;
        
        [CanBeNull] private ValueReference<TValue> currentValue;

        public override void Dispose()
        {
            currentValue?.Dispose();
            base.Dispose();
        }

        public override bool StateLayoutEquals(IComponent other) =>
            other is FieldBase<TValue, TElem> field && field.onValueChanged == onValueChanged;
        
        protected override TElem PrepareElement(TElem target)
        {
            target.SetValueWithoutNotify(currentValue);
            
            target.RegisterValueChangedCallback(OnValueChanged);
            AddCleanup(target, () => target.UnregisterValueChangedCallback(OnValueChanged));
            
            return target;
        }

        protected FieldBase([NotNull] Action<TValue> onValueChanged, TValue initialValue, IManipulator[] manipulators): base(manipulators)
        {
            this.onValueChanged = onValueChanged;
            this.initialValue = initialValue;
        }

        protected override void OnState(CompositionContext context)
        {
            base.OnState(context);
            currentValue = context.RememberRef(initialValue);
        }

        private void OnValueChanged(ChangeEvent<TValue> evt)
        {
            if (EqualityComparer<TValue>.Default.Equals(evt.newValue, evt.previousValue))
                return;

            if (currentValue != null)
                currentValue.Value = evt.newValue;
            
            onValueChanged(evt.newValue);
        }
    }
}