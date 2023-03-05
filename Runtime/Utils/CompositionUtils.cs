using System;
using UI.Li.Common;
using JetBrains.Annotations;
using UnityEngine.UIElements;
using System.Collections.Generic;

using Box = UI.Li.Common.Box;
using Flex = UI.Li.Common.Flex;
using Button = UI.Li.Common.Button;
using TextField = UI.Li.Common.TextField;

namespace UI.Li.Utils
{
    /// <summary>
    /// Utility class aimed to simplify basic use of composables.
    /// </summary>
    /// <remarks>Yeah, I know it's not pretty by we don't have functions outside classes soo...</remarks>
    [PublicAPI] public static class CompositionUtils
    {
        /// <summary>
        /// Returns given compositions with set id.
        /// </summary>
        /// <remarks>Id cannot be overriden later.</remarks>
        /// <param name="id">id</param>
        /// <param name="composition">composition to add id to</param>
        /// <returns></returns>
        [PublicAPI]
        [NotNull]
        public static IComposition WithId(int id, IComposition composition)
        {
            composition.OnBeforeRecompose += ctx => ctx.SetNextEntryId(id);
            
            return composition;
        }

        /// <summary>
        /// Utility for switching between static compositions.
        /// </summary>
        /// <remarks>Gives each composition unique id based on its number in the list.</remarks>
        /// <param name="choice">number of currently selected composition</param>
        /// <param name="compositions">available compositions to choose from</param>
        /// <returns></returns>
        [PublicAPI]
        [NotNull]
        public static IComposition Switch(int choice, [NotNull] params Func<IComposition>[] compositions) =>
            WithId(choice, compositions[choice]());

        /// <summary>
        /// Utility for switching between two static compositions.
        /// </summary>
        /// <remarks>Gives each composition unique id based.</remarks>
        /// <param name="choice">chooses from two compositions</param>
        /// <param name="onTrue">composition chosen when choice is <c>true</c></param>
        /// <param name="onFalse">composition chosen when choice is <c>false</c></param>
        /// <returns></returns>
        [PublicAPI]
        [NotNull]
        public static IComposition Switch(bool choice, [NotNull] Func<IComposition> onTrue, [NotNull] Func<IComposition> onFalse) =>
            choice ? WithId(1, onTrue()) : WithId(2, onFalse());

        /// <summary>
        /// Creates text composition, see <see cref="Common.Text.V(string, Element.Data)"/>.
        /// </summary>
        /// <param name="text">text</param>
        /// <param name="data">additional element data</param>
        /// <returns></returns>
        [PublicAPI]
        [NotNull]
        public static Text Text([NotNull] string text, Element.Data data = new()) =>
            Common.Text.V(text, data);
        
        /// <summary>
        /// Creates button composition, see <see cref="Common.Button.V(Action, IComposition, Element.Data)"/>.
        /// </summary>
        /// <param name="onClick">called when element is clicked</param>
        /// <param name="content">content of the button</param>
        /// <param name="data">additional element data</param>
        /// <returns></returns>
        [PublicAPI]
        [NotNull]
        public static Button Button([NotNull] Action onClick, [NotNull] IComposition content, Element.Data data = new()) =>
            Common.Button.V(onClick, content, data);
        /// <summary>
        /// Creates button composition, see <see cref="Common.Button.V(Action, string, Element.Data)"/>.
        /// </summary>
        /// <param name="onClick">called when element is clicked</param>
        /// <param name="content">content of the button</param>
        /// <param name="data">additional element data</param>
        /// <returns></returns>
        [PublicAPI]
        [NotNull]
        public static Button Button([NotNull] Action onClick, [NotNull] string content, Element.Data data = new()) =>
            Common.Button.V(onClick, content, data);

        /// <summary>
        /// Creates flex composition, see <see cref="Common.Flex.V(IEnumerable{IComposition}, FlexDirection, Element.Data)"/>.
        /// </summary>
        /// <param name="content">content of flex element</param>
        /// <param name="direction">direction of content flow</param>
        /// <param name="data">additional element data</param>
        /// <returns></returns>
        [PublicAPI]
        [NotNull]
        public static Flex Flex([NotNull] IEnumerable<IComposition> content, FlexDirection direction = FlexDirection.Column, Element.Data data = new()) =>
            Common.Flex.V(content, direction, data);

        /// <summary>
        /// Creates text field composition, see <see cref="Common.TextField.V(Action{string}, string, string, bool, Element.Data)"/>.
        /// </summary>
        /// <param name="onValueChanged">called when field content changes</param>
        /// <param name="initialValue">initial text</param>
        /// <param name="tooltip">tooltip</param>
        /// <param name="focused">indices whether element should be focused after render or not</param>
        /// <param name="data">additional element data</param>
        /// <returns></returns>
        [PublicAPI]
        [NotNull]
        public static TextField TextField(
            [NotNull] Action<string> onValueChanged,
            [NotNull] string initialValue = "",
            [NotNull] string tooltip = "",
            bool focused = false,
            Element.Data data = new()
        ) => Common.TextField.V(onValueChanged, initialValue, tooltip, focused, data);

        [PublicAPI]
        [NotNull]
        public static Dropdown Dropdown(
            int initialValue,
            [NotNull] Action<int> onSelectionChanged,
            [NotNull] List<string> options,
            Element.Data data = new()
        ) => Common.Dropdown.V(initialValue, onSelectionChanged, options, data);
        
        [PublicAPI]
        [NotNull]
        public static Dropdown Dropdown<T>(
            int initialValue,
            [NotNull] Action<T> onSelectionChanged,
            Element.Data data = new()
        ) where T : Enum => Common.Dropdown.V(initialValue, onSelectionChanged, data);
        
        [PublicAPI]
        [NotNull]
        public static Box Box(IComposition content = null, Element.Data data = new()) => Common.Box.V(content, data);

        [PublicAPI]
        [NotNull]
        public static IComposition Foldout(
            [NotNull] IComposition header,
            [NotNull] IComposition content,
            bool initiallyOpen = false,
            bool nobToggleOnly = false,
            Element.Data data = new()
        ) => Common.Foldout.V(header, content, initiallyOpen, nobToggleOnly, data);
        
        [PublicAPI]
        [NotNull]
        public static IComposition Foldout(
            [NotNull] string header,
            [NotNull] IComposition content,
            bool initiallyOpen = false,
            bool nobToggleOnly = false,
            Element.Data data = new()
        ) => Common.Foldout.V(header, content, initiallyOpen, nobToggleOnly, data);
    }

    /// <summary>
    /// Utility class used to render compositions.
    /// </summary>
    /// <remarks>Designed to be used from outside composables system to render compositions.</remarks>
    [PublicAPI] public class CompositionRenderer: IDisposable
    {
        private readonly CompositionContext context;
        private readonly IComposition composition;

        /// <summary>
        /// Constructs renderer using given composition.
        /// </summary>
        /// <param name="composition">composition to render</param>
        /// <param name="name">name of context to be displayed in debugger</param>
        [PublicAPI] public CompositionRenderer([NotNull] IComposition composition, string name = "Unnamed")
        {
            this.composition = composition;
            context = new CompositionContext(name);
        }

        /// <summary>
        /// Recomposes composition, <see cref="IComposition.Recompose"/>.
        /// </summary>
        [PublicAPI] public void Update() => composition.Recompose(context);

        /// <summary>
        /// Renders composition, <see cref="IComposition.Render"/>.
        /// </summary>
        /// <returns></returns>
        [PublicAPI] public VisualElement Render() => composition.Render();

        /// <summary>
        /// Updates and renders composition at the same time.
        /// </summary>
        /// <returns></returns>
        [PublicAPI]
        public VisualElement UpdateAndRender()
        {
            Update();
            return Render();
        }
        
        /// <summary>
        /// Disposes of the managed composition and its state.
        /// </summary>
        /// <remarks>Disposed renderer shouldn't be used.</remarks>
        public void Dispose()
        {
            context.Dispose();
            composition.Dispose();
        }
    }
}