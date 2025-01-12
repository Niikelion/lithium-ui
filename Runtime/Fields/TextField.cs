using System;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace UI.Li.Fields
{
    /// <summary>
    /// Component representing <see cref="UnityEngine.UIElements.TextField"/>.
    /// </summary>
    [PublicAPI] public sealed class TextField: TextFieldBase<string, UnityEngine.UIElements.TextField>
    {
        /// <summary>
        /// Constructs <see cref="TextField"/> instance.
        /// </summary>
        /// <param name="onValueChanged">callback invoked every time value is changed</param>
        /// <param name="value">initial value</param>
        /// <param name="placeholder">placeholder</param>
        /// <param name="focused">indicates whether element should be focused after render, currently does nothing</param>
        /// <param name="manipulators">manipulators <seealso cref="IManipulator"/></param>
        /// <returns></returns>
        [NotNull]
        public static TextField V(
            [NotNull] Action<string> onValueChanged,
            [NotNull] string value = "",
            [NotNull] string placeholder = "",
            bool focused = false,
            params IManipulator[] manipulators
        ) => new(onValueChanged, value, placeholder, focused, manipulators);

        public override bool StateLayoutEquals(IComponent other) =>
            other is TextField && base.StateLayoutEquals(other);
        
        private TextField(Action<string> onValueChanged, string value, string placeholder, bool focused, IManipulator[] manipulators): base(onValueChanged, value, placeholder, focused, manipulators) { }
    }
}