using UnityEditor;
using UI.Li.Common;
using UnityEngine.UIElements;

namespace UI.Li.Editor
{
    public class PropertyField: Element
    {
        private readonly SerializedProperty property;
        
        private PropertyField(SerializedProperty property)
        {
            this.property = property;
        }

        protected override VisualElement GetElement(VisualElement source) =>
            new UnityEditor.UIElements.PropertyField(property);
    }
}