using System;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace UI.Li.Common
{
    /// <summary>
    /// Component representing <see cref="UnityEngine.UIElements.Button"/>.
    /// </summary>
    [PublicAPI] public class Button: Element
    {
        [NotNull] private readonly Action onClick;
        [NotNull] private readonly IComponent content;

        /// <summary>
        /// Creates <see cref="Button"/> instance with given content.
        /// </summary>
        /// <param name="onClick">callback invoked every time element is clicked <seealso cref="UnityEngine.UIElements.Button.clicked"/></param>
        /// <param name="content">content of the button</param>
        /// <param name="data">additional element data <seealso cref="Element.Data"/></param>
        /// <returns></returns>
        [NotNull] [Obsolete]
        public static Button V([NotNull] Action onClick, [NotNull] IComponent content, Data data) => new(onClick, content, data);
        /// <summary>
        /// Creates <see cref="Button"/> instance with given text.
        /// </summary>
        /// <param name="onClick">callback invoked every time element is clicked <seealso cref="UnityEngine.UIElements.Button.clicked"/></param>
        /// <param name="content">text of the button</param>
        /// <param name="data">additional element data <seealso cref="Element.Data"/></param>
        /// <returns></returns>
        [NotNull] [Obsolete] 
        public static Button V([NotNull] Action onClick, [NotNull] string content, Data data) => new(onClick, Text.V(content), data);
        
        /// <summary>
        /// Creates <see cref="Button"/> instance with given content.
        /// </summary>
        /// <param name="onClick">callback invoked every time element is clicked <seealso cref="UnityEngine.UIElements.Button.clicked"/></param>
        /// <param name="content">content of the button</param>
        /// <param name="manipulators">manipulators <seealso cref="IManipulator"/></param>
        /// <returns></returns>
        [NotNull]
        public static Button V([NotNull] Action onClick, [NotNull] IComponent content, params IManipulator[] manipulators) => new(onClick, content, manipulators);
        /// <summary>
        /// Creates <see cref="Button"/> instance with given text.
        /// </summary>
        /// <param name="onClick">callback invoked every time element is clicked <seealso cref="UnityEngine.UIElements.Button.clicked"/></param>
        /// <param name="content">text of the button</param>
        /// <param name="manipulators">manipulators <seealso cref="IManipulator"/></param>
        /// <returns></returns>
        [NotNull]
        public static Button V([NotNull] Action onClick, [NotNull] string content, params IManipulator[] manipulators) => new(onClick, Text.V(content), manipulators);

        
        [Obsolete] private Button([NotNull] Action onClick, [NotNull] IComponent content, Data data): base(data)
        {
            this.onClick = onClick;
            this.content = content;
        }

        private Button([NotNull] Action onClick, [NotNull] IComponent content, IManipulator[] manipulators) : base(
            manipulators)
        {
            this.onClick = onClick;
            this.content = content;
        }

        protected override VisualElement GetElement(VisualElement source)
        {
            var element = Use<UnityEngine.UIElements.Button>(source, true);

            element.clicked += onClick;
            
            AddCleanup(element, () => element.clicked -= onClick);
            
            element.Add(content.Render());
            
            return element;
        }

        protected override VisualElement PrepareElement(VisualElement target)
        {
            var ret = base.PrepareElement(target);

            ret.AddToClassList(UnityEngine.UIElements.Button.ussClassName);
            ret.AddToClassList(TextElement.ussClassName);
            
            return ret;
        }

        protected override void OnState(CompositionContext context) => content.Recompose(context);
    }
}