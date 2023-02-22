using System;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace UI.Li.Common
{
    /// <summary>
    /// Composition representing <see cref="UnityEngine.UIElements.Button"/>.
    /// </summary>
    [PublicAPI] public class Button: Element
    {
        [NotNull] private readonly Action onClick;
        [NotNull] private readonly IComposition content;

        /// <summary>
        /// Creates <see cref="Button"/> instance with given content.
        /// </summary>
        /// <param name="onClick">callback invoked every time element is clicked <seealso cref="UnityEngine.UIElements.Button.clicked"/></param>
        /// <param name="content">content of the button</param>
        /// <param name="data">additional element data <seealso cref="Element.Data"/></param>
        /// <returns></returns>
        [PublicAPI] [NotNull] public static Button V([NotNull] Action onClick, [NotNull] IComposition content, Data data = new()) => new(onClick, content, data);
        /// <summary>
        /// Creates <see cref="Button"/> instance with given text.
        /// </summary>
        /// <param name="onClick">callback invoked every time element is clicked <seealso cref="UnityEngine.UIElements.Button.clicked"/></param>
        /// <param name="content">text of the button</param>
        /// <param name="data">additional element data <seealso cref="Element.Data"/></param>
        /// <returns></returns>
        [PublicAPI] [NotNull] public static Button V([NotNull] Action onClick, [NotNull] string content, Data data = new()) => new(onClick, Text.V(content), data);
        
        private Button([NotNull] Action onClick, [NotNull] IComposition content, Data data): base(data)
        {
            this.onClick = onClick;
            this.content = content;
        }

        protected override VisualElement GetElement(VisualElement source)
        {
            var element = Use<UnityEngine.UIElements.Button>(source);

            element.clicked -= onClick;
            element.clicked += onClick;
            
            element.Clear();
            element.Add(content.Render());
            
            return element;
        }

        protected override VisualElement PrepareElement(VisualElement target)
        {
            var ret = base.PrepareElement(target);

            ret.AddToClassList("unity-text-element");
            ret.AddToClassList("unity-button");
            
            return ret;
        }

        protected override void OnState(CompositionContext context) => content.Recompose(context);

        public override void Dispose()
        {
            if (PreviouslyRendered is UnityEngine.UIElements.Button oldElement)
                oldElement.clicked -= onClick;

            base.Dispose();
        }
    }
}