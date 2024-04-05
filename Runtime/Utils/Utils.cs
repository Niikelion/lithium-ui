using System;
using JetBrains.Annotations;

namespace UI.Li.Utils
{
    public static class Utils
    {
        /// <summary>
        /// Utility for switching between static compositions.
        /// </summary>
        /// <remarks>Gives each component unique id based on its number in the list.</remarks>
        /// <param name="choice">number of currently selected component</param>
        /// <param name="compositions">available compositions to choose from</param>
        /// <returns></returns>
        [NotNull]
        public static IComponent Switch(int choice, [NotNull] params Func<IComponent>[] compositions) =>
            SwitchWrapper.V(compositions[choice]?.Invoke(), choice);

        /// <summary>
        /// Utility for switching between two static compositions.
        /// </summary>
        /// <remarks>Gives each component unique id based.</remarks>
        /// <param name="choice">chooses from two compositions</param>
        /// <param name="onTrue">component chosen when choice is <c>true</c></param>
        /// <param name="onFalse">component chosen when choice is <c>false</c></param>
        /// <returns></returns>
        [NotNull]
        public static IComponent Switch(bool choice, Func<IComponent> onTrue,
            Func<IComponent> onFalse) =>
            Switch(choice ? 1 : 0, onFalse, onTrue);

    }
}