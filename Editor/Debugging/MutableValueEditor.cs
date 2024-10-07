using UnityEditor;

using static UI.Li.Editor.Fields;

namespace UI.Li.Editor.Debugging
{
    [CustomPropertyDrawer(typeof(MutableValue<>), true)]
    public class MutableValueEditor: ComposablePropertyDrawer
    {
        protected override bool HideContext => true;
        protected override IComponent Layout(SerializedProperty property) =>
            Property(property.FindPropertyRelative("value"), label: "");
    }
}