using JetBrains.Annotations;
using System;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace UI.Li.Common
{
    [PublicAPI]
    public class ObjectField<T>: Element where T: UnityEngine.Object
    {
        [NotNull] private readonly Action<T> changeCallback;
        private readonly T defaultValue;

        [NotNull] public static
            ObjectField<T> V([NotNull] Action<T> onValueChanged, T defaultValue = null,
                Data data = new()) =>
            new (onValueChanged, defaultValue, data);

        [NotNull]
        public static ObjectField<T> V([NotNull] MutableValue<T> value, Data data = new()) =>
            new(v => value.Value = v, value.Value, data);

        private ObjectField([NotNull] Action<T> onValueChanged, T defaultValue, Data data): base(data)
        {
            changeCallback = onValueChanged;
            this.defaultValue = defaultValue;
        }
        
        protected override VisualElement GetElement(VisualElement source)
        {
            var element = Use<UnityEditor.UIElements.ObjectField>(source);

            element.objectType = typeof(T);
            element.value = defaultValue;
            element.RegisterValueChangedCallback(OnValueChanged);
            AddCleanup(element, () => element.UnregisterValueChangedCallback(OnValueChanged));
            
            return element;
        }

        private void OnValueChanged(ChangeEvent<UnityEngine.Object> evt) =>
            changeCallback(evt.newValue as T);
    }
}