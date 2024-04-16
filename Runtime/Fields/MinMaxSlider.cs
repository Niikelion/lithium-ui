using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Li.Fields
{
    [PublicAPI] public sealed class MinMaxSlider: FieldBase<Vector2, UnityEngine.UIElements.MinMaxSlider>
    {
        private readonly float min, max;

        protected override UnityEngine.UIElements.MinMaxSlider PrepareElement(UnityEngine.UIElements.MinMaxSlider target)
        {
            var elem = base.PrepareElement(target);

            elem.highLimit = max;
            elem.lowLimit = min;
            
            return elem;
        }

        private MinMaxSlider([NotNull] Action<Vector2> onValueChanged, Vector2 initialValue, float min, float max, IManipulator[] manipulators) : base(onValueChanged, initialValue, manipulators)
        {
            this.min = min;
            this.max = max;
        }
    }
}