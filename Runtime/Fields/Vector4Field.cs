using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Li.Fields
{
    [PublicAPI]
    public class Vector4Field: FieldBase<Vector4, UnityEngine.UIElements.Vector4Field>
    {
        [NotNull]
        public static Vector4Field V([NotNull] Action<Vector4> onValueChanged, Vector4 initialValue = default, params IManipulator[] manipulators) =>
            new(onValueChanged, initialValue, manipulators);
        
        private Vector4Field([NotNull] Action<Vector4> onValueChanged, Vector4 initialValue, IManipulator[] manipulators) : base(onValueChanged, initialValue, manipulators) { }
    }
}