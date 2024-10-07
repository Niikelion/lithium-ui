using System.Reflection;
using JetBrains.Annotations;
using UnityEditor;

using static UI.Li.ComponentState;

namespace UI.Li.Editor.Debugging
{
    [CustomPropertyDrawer(typeof(MutableValue<>), true)]
    public class MutableValueEditor: ComposablePropertyDrawer
    {
        protected override bool HideContext => true;
        protected override IComponent Layout(SerializedProperty property) =>
            PropertyField.V(property.FindPropertyRelative("value"));
    }
}