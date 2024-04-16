using System;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace UI.Li.Fields
{
    [PublicAPI] public sealed class FloatField: TextFieldBase<float, UnityEngine.UIElements.FloatField>
    {
        [NotNull]
        public static FloatField V(
            [NotNull] Action<float> onValueChanged,
            float value = 0f,
            [NotNull] string placeholder = "",
            bool focused = false,
            params IManipulator[] manipulators
        ) => new(onValueChanged, value, placeholder, focused, manipulators);
        
        public override bool StateLayoutEquals(IComponent other) =>
            other is FloatField && base.StateLayoutEquals(other);

        private FloatField(
            Action<float> onValueChanged,
            float value,
            string placeholder,
            bool focused,
            IManipulator[] manipulators
        ): base(onValueChanged, value, placeholder, focused, manipulators) {}
    }
}