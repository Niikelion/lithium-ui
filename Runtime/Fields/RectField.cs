using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Li.Fields
{
    [PublicAPI]
    public class RectField: FieldBase<Rect, UnityEngine.UIElements.RectField>
    {
        [NotNull]
        public static RectField V([NotNull] Action<Rect> onValueChanged, Rect initialValue = default, params IManipulator[] manipulators) =>
            new (onValueChanged, initialValue, manipulators);
        
        private RectField([NotNull] Action<Rect> onValueChanged, Rect initialValue, IManipulator[] manipulators) : base(onValueChanged, initialValue, manipulators) { }
    }
}