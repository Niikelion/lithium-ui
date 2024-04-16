using System;
using JetBrains.Annotations;
using UnityEditor;
using UI.Li.Common;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Li.Editor
{
    [PublicAPI] public class PropertyField: Element
    {
        private readonly SerializedProperty property;
        private readonly Action<object> onValueChanged;

        [NotNull]
        public static PropertyField V([NotNull] SerializedProperty property, Action<object> onValueChanged = null, params IManipulator[] manipulators) =>
            new(property, onValueChanged, manipulators);

        protected override VisualElement GetElement(VisualElement source) =>
            new UnityEditor.UIElements.PropertyField();

        protected override VisualElement PrepareElement(VisualElement target)
        {
            var elem = base.PrepareElement(target) as UnityEditor.UIElements.PropertyField;

            elem.BindProperty(property);
            
            Debug.Assert(elem != null);
            
            if (onValueChanged != null)
                elem.RegisterValueChangeCallback(OnValueChanged);
            
            return elem;
        }

        protected PropertyField(SerializedProperty property, Action<object> onValueChanged, IManipulator[] manipulators): base(manipulators)
        {
            this.property = property;
            this.onValueChanged = onValueChanged;
        }

        private void OnValueChanged(SerializedPropertyChangeEvent evt)
        {
            onValueChanged?.Invoke(evt.changedProperty.boxedValue);
        }
    }
}