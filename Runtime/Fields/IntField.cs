using System;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace UI.Li.Fields
{
    [PublicAPI] public sealed class IntField: TextFieldBase<int, IntegerField>
    {
        [NotNull]
        public static IntField V(
            [NotNull] Action<int> onValueChanged,
            int value = 0,
            [NotNull] string placeholder = "",
            bool focused = false,
            params IManipulator[] manipulators
        ) => new(onValueChanged, value, placeholder, focused, manipulators);
        
        public override bool StateLayoutEquals(IComponent other) =>
            other is IntField && base.StateLayoutEquals(other);

        private IntField(
            Action<int> onValueChanged,
            int value,
            string placeholder,
            bool focused,
            IManipulator[] manipulators
        ): base(onValueChanged, value, placeholder, focused, manipulators) {}
    }
}