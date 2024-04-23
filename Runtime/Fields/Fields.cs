using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
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
        /// Creates float field component, see <see cref="Li.Fields.FloatField.V(Action{float}, float, string, bool, IManipulator[])"/>
        /// </summary>
        /// <param name="onValueChanged">called when the field content changes</param>
        /// <param name="initialValue">initial value</param>
        /// <param name="placeholder">placeholder</param>
        /// <param name="focused">indicates whether the element should be focused after the render or not</param>
        /// <param name="manipulators">manipulators</param>
        /// <returns></returns>
        [NotNull]
        public static FloatField FloatField(
            [NotNull] Action<float> onValueChanged,
            float initialValue = 0f,
            [NotNull] string placeholder = "",
            bool focused = false,
            params IManipulator[] manipulators
        ) => Li.Fields.FloatField.V(onValueChanged, initialValue, placeholder, focused, manipulators);
        
        //TODO: IntField
        
        //TODO: IntSlider
        
        //TODO: MinMaxSlider
        
        //TODO: RectField
        
        //TODO: Slider
        
        /// <summary>
        /// Creates text field component, see <see cref="Li.Fields.TextField.V(Action{string}, string, string, bool, IManipulator[])"/>.
        /// </summary>
        /// <param name="onValueChanged">called when the field content changes</param>
        /// <param name="initialValue">initial text</param>
        /// <param name="placeholder">placeholder</param>
        /// <param name="focused">indicates whether the element should be focused after the render or not</param>
        /// <param name="manipulators">manipulators</param>
        /// <returns></returns>
        [NotNull]
        public static TextField TextField(
            [NotNull] Action<string> onValueChanged,
            [NotNull] string initialValue = "",
            [NotNull] string placeholder = "",
            bool focused = false,
            params IManipulator[] manipulators
        ) => Li.Fields.TextField.V(onValueChanged, initialValue, placeholder, focused, manipulators);
        
        /// <summary>
        /// Creates toggle field component, see <see cref="Li.Fields.Toggle.V(Action{bool}, bool, IManipulator[])"/>
        /// </summary>
        /// <param name="onValueChanged">called when the field state changes</param>
        /// <param name="initialValue">initial value</param>
        /// <param name="manipulators">manipulators</param>
        /// <returns></returns>
        [NotNull]
        public static Toggle Toggle(
            [NotNull] Action<bool> onValueChanged,
            bool initialValue = false,
            params IManipulator[] manipulators
        ) => Li.Fields.Toggle.V(onValueChanged, initialValue, manipulators);

        /// <summary>
        /// Creates Vector2 field component, see <see cref="Li.Fields.Vector2Field.V(Action{Vector2}, Vector2, IManipulator[])"/>
        /// </summary>
        /// <param name="onValueChanged">called when the value changes</param>
        /// <param name="initialValue">initial value</param>
        /// <param name="manipulators">manipulators</param>
        /// <returns></returns>
        [NotNull]
        public static Vector2Field Vector2Field(
            [NotNull] Action<Vector2> onValueChanged,
            Vector2 initialValue = default,
            params IManipulator[] manipulators
        ) => Li.Fields.Vector2Field.V(onValueChanged, initialValue, manipulators);

        /// <summary>
        /// Creates Vector3 field component, see <see cref="Li.Fields.Vector3Field.V(Action{Vector3}, Vector3, IManipulator[])"/>
        /// </summary>
        /// <param name="onValueChanged">called when the value changes</param>
        /// <param name="initialValue">initial value</param>
        /// <param name="manipulators">manipulators</param>
        /// <returns></returns>
        [NotNull]
        public static Vector3Field Vector3Field(
            [NotNull] Action<Vector3> onValueChanged,
            Vector3 initialValue = default,
            params IManipulator[] manipulators
        ) => Li.Fields.Vector3Field.V(onValueChanged, initialValue, manipulators);
        
        /// <summary>
        /// Creates Vector3 field component, see <see cref="Li.Fields.Vector4Field.V(Action{Vector4}, Vector4, IManipulator[])"/>
        /// </summary>
        /// <param name="onValueChanged">called when the value changes</param>
        /// <param name="initialValue">initial value</param>
        /// <param name="manipulators">manipulators</param>
        /// <returns></returns>
        [NotNull]
        public static Vector4Field Vector4Field(
            [NotNull] Action<Vector4> onValueChanged,
            Vector3 initialValue = default,
            params IManipulator[] manipulators
        ) => Li.Fields.Vector4Field.V(onValueChanged, initialValue, manipulators);
    }
}