using System;
using System.Text;
using JetBrains.Annotations;
using UnityEditor;
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

        private static string ToLiteral(string input) {
            var literal = new StringBuilder(input.Length + 2);
            foreach (var c in input) {
                switch (c) {
                    case '\"': literal.Append("\\\""); break;
                    case '\\': literal.Append(@"\\"); break;
                    case '\0': literal.Append(@"\0"); break;
                    case '\a': literal.Append(@"\a"); break;
                    case '\b': literal.Append(@"\b"); break;
                    case '\f': literal.Append(@"\f"); break;
                    case '\n': literal.Append(@"\n"); break;
                    case '\r': literal.Append(@"\r"); break;
                    case '\t': literal.Append(@"\t"); break;
                    case '\v': literal.Append(@"\v"); break;
                    default:
                        // ASCII printable character
                        if (c >= 0x20 && c <= 0x7e) {
                            literal.Append(c);
                            // As UTF16 escaped character
                        } else {
                            literal.Append(@"\u");
                            literal.Append(((int)c).ToString("x4"));
                        }
                        break;
                }
            }
            
            return literal.ToString();
        }
        
        public override string ToString() => $"Text \"{ToLiteral(text)}\"";

        public override bool StateLayoutEquals(IComponent other) => other is Text;

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