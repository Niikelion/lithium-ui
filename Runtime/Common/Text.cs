using System;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace UI.Li.Common
{
    /// <summary>
    /// Component representing <see cref="UnityEngine.UIElements.Label"/>.
    /// </summary>
    [PublicAPI] public sealed class Text: Element
    {
        [NotNull] private readonly string text;
        private readonly string tooltip;

        /// <summary>
        /// Creates <see cref="Text"/> instance with given text.
        /// </summary>
        /// <param name="text">text to be displayed</param>
        /// <param name="data">additional element data <seealso cref="Element.Data"/></param>
        /// <returns></returns>
        [NotNull] [Obsolete]
        public static Text V([NotNull] string text, Data data) =>
            new(text, data);
        
        /// <summary>
        /// Creates <see cref="Text"/> instance with given text.
        /// </summary>
        /// <param name="text">text to be displayed</param>
        /// <param name="manipulators">manipulators <seealso cref="IManipulator"/></param>
        /// <returns></returns>
        [NotNull]
        public static Text V([NotNull] string text, params IManipulator[] manipulators) =>
            new(text, manipulators);
        
        [Obsolete] private Text([NotNull] string text, Data data) : base(data) => this.text = text;

        private Text([NotNull] string text, IManipulator[] manipulators) : base(manipulators) => this.text = text;

        protected override VisualElement PrepareElement(VisualElement target)
        {
            var ret = base.PrepareElement(target);
            
            ret.AddToClassList(Label.ussClassName);
            ret.AddToClassList(TextElement.ussClassName);
            
            return ret;
        }

        protected override VisualElement GetElement(VisualElement source)
        {
            var element = Use<Label>(source, true);
            element.text = text;

            return element;
        }
    }
}