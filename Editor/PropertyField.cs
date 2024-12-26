using System;
using JetBrains.Annotations;
using UnityEditor;
using UI.Li.Common;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace UI.Li.Editor
{
    [PublicAPI] public class PropertyField: Element<UnityEditor.UIElements.PropertyField>
    {
        private readonly SerializedProperty property;
        private readonly Action<object> onValueChanged;
        private readonly Action<object> onValueApplied;
        private readonly string label;

        [NotNull]
        public static PropertyField V([NotNull] SerializedProperty property, Action<object> onValueChanged = null, Action<object> onValueApplied = null, string label = null, params IManipulator[] manipulators) =>
            new(property, onValueChanged, onValueApplied, label, manipulators);

        protected override UnityEditor.UIElements.PropertyField PrepareElement(UnityEditor.UIElements.PropertyField target)
        {
            var elem = base.PrepareElement(target);
            
            elem.label = label;
            elem.BindProperty(property);

            CompositionContext.ElementUserData.AppendCleanupAction(elem, () => elem.Unbind());
            
            if (onValueChanged != null)
                elem.RegisterValueChangeCallback(OnValueChanged);

            if (onValueApplied != null)
            {
                elem.RegisterCallback<AttachToPanelEvent>(OnAttachedToPanel);

                CompositionContext.ElementUserData.AppendCleanupAction(elem, () =>
                    elem.UnregisterCallback<AttachToPanelEvent>(OnAttachedToPanel));
                
                void OnAttachedToPanel(AttachToPanelEvent evt)
                {
                    //
                }
            }
            
            return elem;
        }

        protected PropertyField(SerializedProperty property, Action<object> onValueChanged, Action<object> onValueApplied, string label, IManipulator[] manipulators): base(manipulators)
        {
            this.property = property;
            this.onValueChanged = onValueChanged;
            this.onValueApplied = onValueApplied;
            this.label = label;
        }

        private void OnValueChanged(SerializedPropertyChangeEvent evt) =>
            onValueChanged?.Invoke(evt.changedProperty.boxedValue);

        private void OnValueApplied(SerializedProperty prop) =>
            onValueApplied?.Invoke(prop.boxedValue);
    }
}