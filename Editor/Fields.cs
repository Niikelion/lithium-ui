using System;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine.UIElements;

namespace UI.Li.Editor
{
    [PublicAPI] public static class Fields
    {
        public static IComponent Property(SerializedProperty property, Action<object> onValueChanged = null, string label = null) =>
            PropertyField.V(property, onValueChanged, label);
        public static IComponent Inspector([NotNull] UnityEditor.Editor editor, params IManipulator[] manipulators) =>
            DefaultInspector.V(editor, manipulators);
    }
}