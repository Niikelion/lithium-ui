using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace UI.Li.Fields
{
    [PublicAPI] public static class Fields
    {
        /// <summary>
        /// Creates dropdown field component, see <see cref="Li.Fields.Dropdown.V(int, Action{int}, List{string}, IManipulator[])"/>.
        /// </summary>
        /// <param name="initialValue">number of initially selected option starting from 0</param>
        /// <param name="onSelectionChanged">selection changed callback</param>
        /// <param name="options">displayed options</param>
        /// <param name="manipulators">manipulators</param>
        /// <returns></returns>
        [NotNull]
        public static Dropdown Dropdown(
            int initialValue,
            [NotNull] Action<int> onSelectionChanged,
            [NotNull] List<string> options,
            params IManipulator[] manipulators
        ) => Li.Fields.Dropdown.V(initialValue, onSelectionChanged, options, manipulators);
        
        /// <summary>
        /// Creates dropdown field component, see <see cref="Li.Fields.Dropdown.V{T}(T, Action{T}, IManipulator[])"/>.
        /// </summary>
        /// <param name="initialValue">enum option selected by default</param>
        /// <param name="onSelectionChanged">on selection changed callback</param>
        /// <param name="manipulators">manipulators</param>
        /// <typeparam name="T">enum to be used as in a dropdown</typeparam>
        /// <returns></returns>
        [NotNull]
        public static Dropdown Dropdown<T>(
            T initialValue,
            [NotNull] Action<T> onSelectionChanged,
            params IManipulator[] manipulators
        ) where T : Enum => Li.Fields.Dropdown.V(initialValue, onSelectionChanged, manipulators);
        
        /// <summary>
        /// Creates text field component, see <see cref="Li.Fields.TextField.V(Action{string}, string, string, bool, IManipulator[])"/>.
        /// </summary>
        /// <param name="onValueChanged">called when field content changes</param>
        /// <param name="initialValue">initial text</param>
        /// <param name="tooltip">tooltip</param>
        /// <param name="focused">indices whether element should be focused after render or not</param>
        /// <param name="manipulators">manipulators</param>
        /// <returns></returns>
        [NotNull]
        public static TextField TextField(
            [NotNull] Action<string> onValueChanged,
            [NotNull] string initialValue = "",
            [NotNull] string tooltip = "",
            bool focused = false,
            params IManipulator[] manipulators
        ) => Li.Fields.TextField.V(onValueChanged, initialValue, tooltip, focused, manipulators);
        
        /// <summary>
        /// Creates toggle field component, see <see cref="Li.Fields.Toggle.V(Action{bool}, bool, IManipulator[])"/>
        /// </summary>
        /// <param name="onValueChanged">called when field state changes</param>
        /// <param name="initialValue">initial value</param>
        /// <param name="manipulators">manipulators</param>
        /// <returns></returns>
        [NotNull]
        public static Toggle Toggle(
            [NotNull] Action<bool> onValueChanged,
            bool initialValue = false,
            params IManipulator[] manipulators
        ) => Li.Fields.Toggle.V(onValueChanged, initialValue, manipulators);
    }
}