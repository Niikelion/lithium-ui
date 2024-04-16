using System;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace UI.Li.Fields
{
    [PublicAPI] public sealed class IntSlider: FieldBase<int, SliderInt>
    {
        private readonly int min, max;

        [NotNull]
        public static IntSlider V([NotNull] Action<int> onValueChanged, int initialValue = 0, int minValue = 0, int maxValue = 1, params IManipulator[] manipulators) =>
            new(onValueChanged, initialValue, minValue, maxValue, manipulators);

        public override bool StateLayoutEquals(IComponent other) =>
            other is IntSlider && base.StateLayoutEquals(other);

        protected override SliderInt PrepareElement(SliderInt target)
        {
            var elem = base.PrepareElement(target);

            elem.highValue = max;
            elem.lowValue = min;

            return elem;
        }

        private IntSlider([NotNull] Action<int> onValueChanged, int initialValue, int min, int max, IManipulator[] manipulators) : base(onValueChanged, initialValue, manipulators)
        {
            this.min = min;
            this.max = max;
        }
    }
}