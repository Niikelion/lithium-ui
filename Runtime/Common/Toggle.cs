using JetBrains.Annotations;
using System;
using UnityEngine.UIElements;

namespace UI.Li.Common
{
    public class Toggle: Element
    {
        [NotNull] private readonly Action<bool> onValueChanged;
        private readonly bool initialValue;

        [NotNull]
        public static Toggle V([NotNull] Action<bool> onValueChanged, bool initialValue = false, Data data = new()) =>
            new (onValueChanged, initialValue, data);
        
        protected override VisualElement GetElement(VisualElement source)
        {
            var element = Use<UnityEngine.UIElements.Toggle>(source);
            
            element.value = initialValue;
            element.RegisterValueChangedCallback(OnValueChanged);
            AddCleanup(element, () => element.UnregisterValueChangedCallback(OnValueChanged));
            
            return element;
        }

        protected override VisualElement PrepareElement(VisualElement target)
        {
            var elem = base.PrepareElement(target);

            elem.AddToClassList(UnityEngine.UIElements.Toggle.ussClassName);
            elem.AddToClassList(BaseField<bool>.ussClassName);

            return elem;
        }

        private void OnValueChanged(ChangeEvent<bool> evt) => onValueChanged(evt.newValue);
        
        private Toggle([NotNull] Action<bool> onValueChanged, bool initialValue, Data data): base(data)
        {
            this.onValueChanged = onValueChanged;
            this.initialValue = initialValue;
        }
    }
}