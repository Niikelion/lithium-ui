using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Li.Fields
{
    [PublicAPI]
    public class Vector2Field: FieldBase<Vector2, UnityEngine.UIElements.Vector2Field>
    {
        [NotNull]
        public static Vector2Field V([NotNull] Action<Vector2> onValueChanged, Vector2 initialValue = default, params IManipulator[] manipulators) =>
            new (onValueChanged, initialValue, manipulators);
        
        private Vector2Field([NotNull] Action<Vector2> onValueChanged, Vector2 initialValue, IManipulator[] manipulators) : base(onValueChanged, initialValue, manipulators) { }
    }
}