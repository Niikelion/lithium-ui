using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Li.Fields
{
    [PublicAPI]
    public class Vector3Field: FieldBase<Vector3, UnityEngine.UIElements.Vector3Field>
    {
        [NotNull]
        public static Vector3Field V([NotNull] Action<Vector3> onValueChanged, Vector3 initialValue = default, params IManipulator[] manipulators) =>
            new(onValueChanged, initialValue, manipulators);
        
        private Vector3Field([NotNull] Action<Vector3> onValueChanged, Vector3 initialValue, IManipulator[] manipulators) : base(onValueChanged, initialValue, manipulators) { }
    }
}