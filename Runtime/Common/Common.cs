using System;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace UI.Li.Common
{
    [PublicAPI] public static class Common
    {
        /// <summary>
        /// Creates button component, see <see cref="Li.Common.Button.V(Action, IComponent, IManipulator[])"/>.
        /// </summary>
        /// <param name="onClick">called when element is clicked</param>
        /// <param name="content">content of the button</param>
        /// <param name="manipulators">manipulators</param>
        /// <returns></returns>
        [NotNull]
        public static Button Button([NotNull] Action onClick, [NotNull] IComponent content, params IManipulator[] manipulators) =>
            Li.Common.Button.V(onClick, content, manipulators);
        
        /// <summary>
        /// Creates button component, see <see cref="Li.Common.Button.V(Action, string, IManipulator[])"/>.
        /// </summary>
        /// <param name="onClick">called when element is clicked</param>
        /// <param name="content">content of the button</param>
        /// <param name="manipulators">manipulators</param>
        /// <returns></returns>
        [NotNull]
        public static Button Button([NotNull] Action onClick, [NotNull] string content, params IManipulator[] manipulators) =>
            Li.Common.Button.V(onClick, content, manipulators);
        
        /// <summary>
        /// Creates text component, see <see cref="Li.Common.Text.V(string, IManipulator[])"/>.
        /// </summary>
        /// <param name="text">text</param>
        /// <param name="manipulators">manipulators</param>
        /// <returns></returns>
        [NotNull]
        public static Text Text([NotNull] string text, params IManipulator[] manipulators) =>
            Li.Common.Text.V(text, manipulators);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="component"></param>
        /// <param name="isStatic"></param>
        /// <returns></returns>
        [NotNull, Obsolete]
        public static Component Comp([NotNull] Component.OldStatefulComponent component, bool isStatic = false) => new (component, isStatic);
    }
}