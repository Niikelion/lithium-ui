using System;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace UI.Li.Fields
{
    [PublicAPI] public sealed class Slider: FieldBase<float, UnityEngine.UIElements.Slider>
    {
        private readonly float min, max;
        
        [NotNull]
        public static Slider V([NotNull] Action<float> onValueChanged, float initialValue = 0f, float minValue = 0f, float maxValue = 1f, params IManipulator[] manipulators) =>
            new(onValueChanged, initialValue, minValue, maxValue, manipulators);

        public override bool StateLayoutEquals(IComponent other) =>
            other is Slider && base.StateLayoutEquals(other);

        protected override UnityEngine.UIElements.Slider PrepareElement(UnityEngine.UIElements.Slider target)
        {
            var elem = base.PrepareElement(target);

            elem.highValue = max;
            elem.lowValue = min;
            
            return elem;
        }

        private Slider([NotNull] Action<float> onValueChanged, float initialValue, float min, float max, IManipulator[] manipulators) : base(onValueChanged, initialValue, manipulators)
        {
            this.min = min;
            this.max = max;
        }
    }
}