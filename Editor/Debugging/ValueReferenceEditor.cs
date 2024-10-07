using UnityEditor;

using static UI.Li.Editor.Fields;

namespace UI.Li.Editor.Debugging
{
    [CustomPropertyDrawer(typeof(ValueReference<>), true)]
    public class ValueReferenceEditor: ComposablePropertyDrawer
    {
        protected override bool HideContext => true;
        protected override IComponent Layout(SerializedProperty property) =>
            Property(property.FindPropertyRelative("Value"), label: "");
    }
}