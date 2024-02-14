using System;
using UI.Li.Common;
using JetBrains.Annotations;
using UnityEngine.UIElements;
using System.Collections.Generic;

using Box = UI.Li.Common.Box;
using Flex = UI.Li.Common.Flex;
using Button = UI.Li.Common.Button;
using Foldout = UI.Li.Common.Foldout;
using TextField = UI.Li.Common.TextField;
using Toggle =UI.Li.Common.Toggle;

namespace UI.Li.Utils
{
    /// <summary>
    /// Utility class aimed to simplify basic use of components.
    /// </summary>
    /// <remarks>Yeah, I know it's not pretty by we don't have functions outside classes soo...</remarks>
    [PublicAPI] public static class CompositionUtils
    {
        /// <summary>
        /// Returns given compositions with set id.
        /// </summary>
        /// <remarks>Id cannot be overriden later.</remarks>
        /// <param name="id">id</param>
        /// <param name="component">component to add id to</param>
        /// <returns></returns>
        [PublicAPI]
        [NotNull]
        public static IComponent WithId(int id, IComponent component)
        {
            component.OnBeforeRecompose += ctx => ctx.SetNextEntryId(id);
            
            return component;
        }

        /// <summary>
        /// Utility for switching between static compositions.
        /// </summary>
        /// <remarks>Gives each component unique id based on its number in the list.</remarks>
        /// <param name="choice">number of currently selected component</param>
        /// <param name="compositions">available compositions to choose from</param>
        /// <returns></returns>
        [PublicAPI]
        [NotNull]
        public static IComponent Switch(int choice, [NotNull] params Func<IComponent>[] compositions) =>
            WithId(choice, compositions[choice]());

        /// <summary>
        /// Utility for switching between two static compositions.
        /// </summary>
        /// <remarks>Gives each component unique id based.</remarks>
        /// <param name="choice">chooses from two compositions</param>
        /// <param name="onTrue">component chosen when choice is <c>true</c></param>
        /// <param name="onFalse">component chosen when choice is <c>false</c></param>
        /// <returns></returns>
        [PublicAPI]
        [NotNull]
        public static IComponent Switch(bool choice, [NotNull] Func<IComponent> onTrue, [NotNull] Func<IComponent> onFalse) =>
            choice ? WithId(1, onTrue()) : WithId(2, onFalse());

        /// <summary>
        /// Creates text component, see <see cref="Common.Text.V(string, Element.Data)"/>.
        /// </summary>
        /// <param name="text">text</param>
        /// <param name="data">additional element data</param>
        /// <returns></returns>
        [PublicAPI]
        [NotNull]
        public static Text Text([NotNull] string text, Element.Data data = new()) =>
            Common.Text.V(text, data);
        
        /// <summary>
        /// Creates button component, see <see cref="Common.Button.V(Action, IComponent, Element.Data)"/>.
        /// </summary>
        /// <param name="onClick">called when element is clicked</param>
        /// <param name="content">content of the button</param>
        /// <param name="data">additional element data</param>
        /// <returns></returns>
        [PublicAPI]
        [NotNull]
        public static Button Button([NotNull] Action onClick, [NotNull] IComponent content, Element.Data data = new()) =>
            Common.Button.V(onClick, content, data);
        /// <summary>
        /// Creates button component, see <see cref="Common.Button.V(Action, string, Element.Data)"/>.
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
        /// Creates flex component, see <see cref="Common.Flex.V(IEnumerable{IComponent}, FlexDirection, Element.Data)"/>.
        /// </summary>
        /// <param name="content">content of flex element</param>
        /// <param name="direction">direction of content flow</param>
        /// <param name="data">additional element data</param>
        /// <returns></returns>
        [PublicAPI]
        [NotNull]
        public static Flex Flex([NotNull] IEnumerable<IComponent> content, FlexDirection direction = FlexDirection.Column, Element.Data data = new()) =>
            Common.Flex.V(content, direction, data);

        /// <summary>
        /// Creates text field component, see <see cref="Common.TextField.V(Action{string}, string, string, bool, Element.Data)"/>.
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

        /// <summary>
        /// Creates dropdown field component, see <see cref="Common.Dropdown.V(int, Action{int}, List{string}, Element.Data)"/>.
        /// </summary>
        /// <param name="initialValue">number of initially selected option starting from 0</param>
        /// <param name="onSelectionChanged">selection changed callback</param>
        /// <param name="options">displayed options</param>
        /// <param name="data">additional element data</param>
        /// <returns></returns>
        [PublicAPI]
        [NotNull]
        public static Dropdown Dropdown(
            int initialValue,
            [NotNull] Action<int> onSelectionChanged,
            [NotNull] List<string> options,
            Element.Data data = new()
        ) => Common.Dropdown.V(initialValue, onSelectionChanged, options, data);
        
        /// <summary>
        /// Creates dropdown field component, see <see cref="Common.Dropdown.V{T}(T, Action{T}, Element.Data)"/>.
        /// </summary>
        /// <param name="initialValue">enum option selected by default</param>
        /// <param name="onSelectionChanged">on selection changed callback</param>
        /// <param name="data">additional element data</param>
        /// <typeparam name="T">enum to be used as in a dropdown</typeparam>
        /// <returns></returns>
        [PublicAPI]
        [NotNull]
        public static Dropdown Dropdown<T>(
            T initialValue,
            [NotNull] Action<T> onSelectionChanged,
            Element.Data data = new()
        ) where T : Enum => Common.Dropdown.V(initialValue, onSelectionChanged, data);
        
        /// <summary>
        /// Creates box component, see <see cref="Common.Box.V(IComponent, Element.Data)"/>.
        /// </summary>
        /// <param name="content">content of the box</param>
        /// <param name="data">additional element data</param>
        /// <returns></returns>
        [PublicAPI]
        [NotNull]
        public static Box Box(IComponent content = null, Element.Data data = new()) => Common.Box.V(content, data);

        /// <summary>
        /// Creates foldout component, see <see cref="Common.Foldout.V(IComponent, IComponent, bool, bool, Common.Foldout.HeaderContainer, Common.Foldout.ContentContainer, Element.Data, Func{bool, Action, IComponent})"/>.
        /// </summary>
        /// <param name="header">header of the foldout</param>
        /// <param name="content">content of the foldout</param>
        /// <param name="initiallyOpen">should be open by default</param>
        /// <param name="nobToggleOnly">if true only toggle when clicking the nob, use whole header otherwise</param>
        /// <param name="headerContainer">container used to render header</param>
        /// <param name="contentContainer">container used to render content</param>
        /// <param name="data">additional element data</param>
        /// <returns></returns>
        [PublicAPI]
        [NotNull]
        public static IComponent Foldout(
            [NotNull] IComponent header,
            [NotNull] IComponent content,
            bool initiallyOpen = false,
            bool nobToggleOnly = false,
            Foldout.HeaderContainer headerContainer = null,
            Foldout.ContentContainer contentContainer = null,
            Element.Data data = new()
        ) => Common.Foldout.V(header, content, initiallyOpen, nobToggleOnly, headerContainer, contentContainer, data);
        
        /// <summary>
        /// Creates foldout component, see <see cref="Common.Foldout.V(string, IComponent, bool, bool, Common.Foldout.HeaderContainer, Common.Foldout.ContentContainer, Element.Data, Func{bool, Action, IComponent})"/>.
        /// </summary>
        /// <param name="header">header text</param>
        /// <param name="content">content of the foldout</param>
        /// <param name="initiallyOpen">should be open by default</param>
        /// <param name="nobToggleOnly">if true only toggle when clicking the nob, us whole header otherwise</param>
        /// <param name="headerContainer">container used to render header</param>
        /// <param name="contentContainer">container used to render content</param>
        /// <param name="data">additional element data</param>
        /// <returns></returns>
        [PublicAPI]
        [NotNull]
        public static IComponent Foldout(
            [NotNull] string header,
            [NotNull] IComponent content,
            bool initiallyOpen = false,
            bool nobToggleOnly = false,
            Foldout.HeaderContainer headerContainer = null,
            Foldout.ContentContainer contentContainer = null,
            Element.Data data = new()
        ) => Common.Foldout.V(header, content, initiallyOpen, nobToggleOnly, headerContainer, contentContainer, data);

        /// <summary>
        /// Creates split area component, see <see cref="Common.SplitArea.V(IComponent, IComponent, TwoPaneSplitViewOrientation, float, bool, Element.Data)"/>.
        /// </summary>
        /// <param name="mainContent">main area</param>
        /// <param name="secondaryContent">secondary area</param>
        /// <param name="orientation">orientation</param>
        /// <param name="initialSize">initial size of main area</param>
        /// <param name="reverse">places main area at the end of container</param>
        /// <param name="data">additional element data</param>
        /// <returns></returns>
        [PublicAPI]
        [NotNull]
        public static SplitArea SplitArea(
            [NotNull] IComponent mainContent,
            [NotNull] IComponent secondaryContent,
            TwoPaneSplitViewOrientation orientation = TwoPaneSplitViewOrientation.Horizontal,
            float initialSize = 0,
            bool reverse = false,
            Element.Data data = new()
        ) => Common.SplitArea.V(mainContent, secondaryContent, orientation, initialSize, reverse, data);

        [PublicAPI]
        [NotNull]
        public static Toggle Toggle(
            [NotNull] Action<bool> onValueChanged,
            bool initialValue = false,
            Element.Data data = new()
        ) => Common.Toggle.V(onValueChanged, initialValue, data);

        [PublicAPI]
        [NotNull]
        public static Scroll Scroll(
            IComponent content,
            ScrollViewMode mode = ScrollViewMode.Vertical,
            Action<float, Scroll.Orientation> onScroll = null,
            Element.Data data = new()
        ) => Common.Scroll.V(content, mode, onScroll, data);
    }

    /// <summary>
    /// Utility class used to render components.
    /// </summary>
    /// <remarks>Designed to be used from outside Lithium system to render components.</remarks>
    [PublicAPI] public class ComponentRenderer: IDisposable
    {
        private readonly CompositionContext context;
        private readonly IComponent component;

        /// <summary>
        /// Constructs renderer using given component.
        /// </summary>
        /// <param name="component">component to render</param>
        /// <param name="name">name of context to be displayed in debugger</param>
        [PublicAPI] public ComponentRenderer([NotNull] IComponent component, string name = "Unnamed")
        {
            this.component = component;
            context = new CompositionContext(name);
        }

        /// <summary>
        /// Recomposes component, <see cref="IComponent.Recompose"/>.
        /// </summary>
        [PublicAPI] public void Update() => component.Recompose(context);

        /// <summary>
        /// Renders component, <see cref="IComponent.Render"/>.
        /// </summary>
        /// <returns></returns>
        [PublicAPI] public VisualElement Render() => component.Render();

        /// <summary>
        /// Updates and renders component at the same time.
        /// </summary>
        /// <returns></returns>
        [PublicAPI]
        public VisualElement UpdateAndRender()
        {
            Update();
            return Render();
        }
        
        /// <summary>
        /// Disposes of the managed component and its state.
        /// </summary>
        /// <remarks>Disposed renderer shouldn't be used.</remarks>
        public void Dispose()
        {
            context.Dispose();
            component.Dispose();
        }
    }
}