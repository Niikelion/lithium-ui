using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace UI.Li.Common
{
    /// <summary>
    /// Composition representing <see cref="UnityEngine.UIElements.Label"/>.
    /// </summary>
    [PublicAPI] public sealed class Text: Element
    {
        private readonly string text;

        /// <summary>
        /// Creates <see cref="Text"/> instance with given text.
        /// </summary>
        /// <param name="text">text to be displayed</param>
        /// <param name="data">additional element data <seealso cref="Element.Data"/></param>
        /// <returns></returns>
        [PublicAPI]
        [NotNull]
        public static Text V([NotNull] string text, Data data = new()) =>
            new(text, data);
        
        private Text([NotNull] string text, Data data) : base(data)
        {
            this.text = text;
        }

        protected override VisualElement PrepareElement(VisualElement target)
        {
            var ret = base.PrepareElement(target);
            
            ret.AddToClassList("unity-text-element");
            ret.AddToClassList("unity-label");
            
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