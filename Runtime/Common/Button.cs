using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UI.Li.Utils.Continuations;
using UnityEngine.UIElements;

using UIButton = UnityEngine.UIElements.Button;

namespace UI.Li.Common
{
    /// <summary>
    /// Component representing <see cref="UnityEngine.UIElements.Button"/>.
    /// </summary>
    [PublicAPI] public class Button: Element<UIButton>
    {
        [NotNull] private readonly IComponent content;
        [NotNull] private readonly Action onClick;

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

        public override bool StateLayoutEquals(IComponent other) => other is Button;

        private Button([NotNull] Action onClick, [NotNull] IComponent content, IEnumerable<IManipulator> manipulators) : base(manipulators)
        {
            this.content = content;
            this.onClick = onClick;
        }

        protected override UIButton PrepareElement(UIButton button)
        {
            var contentElement = content.Render();

            button.clicked += onClick;
            CompositionContext.ElementUserData.AppendCleanupAction(button, () => button.clicked -= onClick);
            
            if (button.childCount == 1 && button[0] == contentElement) return button;
            
            button.Clear();
            button.Add(content.Render());

            return button;
        }

        protected override void OnState(CompositionContext context) => content.Recompose(context);
    }
}