using System;
using JetBrains.Annotations;
using UnityEditor;
using UI.Li.Common;
using Unity.Properties;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace UI.Li.Editor
{
    [PublicAPI] public class PropertyField: Element<UnityEditor.UIElements.PropertyField>
    {
        private readonly SerializedProperty property;
        private readonly Action<object> onValueChanged;
        private readonly string label;

        [NotNull]
        public static PropertyField V([NotNull] SerializedProperty property, Action<object> onValueChanged = null, string label = null, params IManipulator[] manipulators) =>
            new(property, onValueChanged, label, manipulators);

        protected override UnityEditor.UIElements.PropertyField PrepareElement(UnityEditor.UIElements.PropertyField target)
        {
            var elem = base.PrepareElement(target);
            
            elem.label = label;
            elem.BindProperty(property);

            CompositionContext.ElementUserData.AppendCleanupAction(elem, () => elem.Unbind());
            
            if (onValueChanged != null)
                elem.RegisterValueChangeCallback(OnValueChanged);

            return elem;
        }

        protected PropertyField(SerializedProperty property, Action<object> onValueChanged, string label, IManipulator[] manipulators): base(manipulators)
        {
            this.property = property;
            this.onValueChanged = onValueChanged;
            this.label = label;
        }

        private void OnValueChanged(SerializedPropertyChangeEvent evt) =>
            onValueChanged?.Invoke(evt.changedProperty.boxedValue);
    }
}