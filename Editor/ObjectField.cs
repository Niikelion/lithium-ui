using JetBrains.Annotations;
using System;
using UI.Li.Common;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace UI.Li.Editor
{
    [PublicAPI]
    public sealed class ObjectField<T>: Element where T: UnityEngine.Object
    {
        [NotNull] private readonly Action<T> changeCallback;
        private readonly T defaultValue;

        [NotNull]
        public static ObjectField<T> V(
            [NotNull] Action<T> onValueChanged,
            T defaultValue = null,
            params IManipulator[] manipulators
            ) => new (onValueChanged, defaultValue, manipulators);

        [NotNull]
        public static ObjectField<T> V([NotNull] MutableValue<T> value, params IManipulator[] manipulators) =>
            new(v => value.Value = v, value.Value, manipulators);
        
        private ObjectField([NotNull] Action<T> onValueChanged, T defaultValue, IManipulator[] manipulators): base(manipulators)
        {
            changeCallback = onValueChanged;
            this.defaultValue = defaultValue;
        }
        
        protected override VisualElement GetElement(VisualElement source)
        {
            var element = Use<ObjectField>(source);

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