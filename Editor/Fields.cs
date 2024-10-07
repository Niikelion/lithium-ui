using System;
using JetBrains.Annotations;
using UnityEditor;

namespace UI.Li.Editor
{
    [PublicAPI] public static class Fields
    {
        public static IComponent Property(SerializedProperty property, Action<object> onValueChanged = null, string label = null) =>
            PropertyField.V(property, onValueChanged, label);
    }
}