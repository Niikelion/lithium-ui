using System;
using JetBrains.Annotations;
using UnityEngine;

namespace UI.Li.Utils
{
    [PublicAPI] public static class Utils
    {
        /// <summary>
        /// Utility for switching between static compositions.
        /// </summary>
        /// <remarks>Gives each component unique id based on its number in the list.</remarks>
        /// <param name="choice">number of currently selected component</param>
        /// <param name="compositions">available compositions to choose from</param>
        /// <returns></returns>
        public static SwitchWrapper Switch(int choice, [NotNull] params Func<IComponent>[] compositions) =>
            SwitchWrapper.V(compositions[choice]?.Invoke(), choice);

        /// <summary>
        /// Utility for switching between two static compositions.
        /// </summary>
        /// <remarks>Gives each component unique id based.</remarks>
        /// <param name="choice">chooses from two compositions</param>
        /// <param name="onTrue">component chosen when choice is <c>true</c></param>
        /// <param name="onFalse">component chosen when choice is <c>false</c></param>
        /// <returns></returns>
        public static SwitchWrapper Switch(bool choice, Func<IComponent> onTrue,
            Func<IComponent> onFalse) =>
            Switch(choice ? 1 : 0, onFalse, onTrue);

        /// <summary>
        /// Utility for enabling part of the layout depending on some condition.
        /// </summary>
        /// <param name="condition">condition that determines whether the component is added or not</param>
        /// <param name="onTrue">component to be added</param>
        /// <returns></returns>
        public static SwitchWrapper If(bool condition, [NotNull] Func<IComponent> onTrue) =>
            Switch(condition, onTrue, null);

        /// <summary>
        /// Utility for enabling part of the layout when value is not null.
        /// </summary>
        /// <param name="value">value than will bi transformed into a component</param>
        /// <param name="onNotNull">function that creates component from value, assuming value is not null</param>
        /// <param name="onNull">function that creates component assuming value is null</param>
        /// <returns></returns>
        public static SwitchWrapper Let<T>(T value, [CanBeNull] NonNullableTransformer<T> onNotNull, [CanBeNull] Func<IComponent> onNull = null) where T: class =>
            Switch(value != null, () =>
            {
                Debug.Assert(value != null);
                
                return onNotNull?.Invoke(value);
            }, onNull);
        
        public delegate IComponent NonNullableTransformer<in T>([NotNull] T value);
        
        /// <summary>
        /// Returns given composition with set id.
        /// </summary>
        /// <remarks>Id cannot be overriden later.</remarks>
        /// <param name="id">id</param>
        /// <param name="component">component to add id to</param>
        /// <returns></returns>
        [NotNull]
        public static IdWrapper WithId(int id, [NotNull] IComponent component) => IdWrapper.V(component, id);
        
        /// <summary>
        /// Sets id for element.
        /// </summary>
        /// <remarks>Id cannot be overriden later.</remarks>
        /// <param name="obj">target component</param>
        /// <param name="id">id</param>
        /// <returns></returns>
        [NotNull]
        public static IdWrapper Id([NotNull] this IComponent obj, int id) => WithId(id, obj);
    }
}