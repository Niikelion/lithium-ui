using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace UI.Li.Common.Layout
{
    [PublicAPI] public static class Layout
    {        
        /// <summary>
        /// Creates box component, see <see cref="Li.Common.Layout.Box.V(IComponent, IManipulator[])"/>.
        /// </summary>
        /// <param name="content">content of the box</param>
        /// <param name="manipulators">manipulators</param>
        /// <returns></returns>
        [NotNull]
        public static Box Box(IComponent content = null, params IManipulator[] manipulators) =>
            Li.Common.Layout.Box.V(content, manipulators);
        
        /// <summary>
        /// Creates flex component, see <see cref="Li.Common.Layout.Flex.V(IEnumerable{IComponent}, FlexDirection, IManipulator[])"/>.
        /// </summary>
        /// <param name="content">content of the flex element</param>
        /// <param name="direction">direction of content flow</param>
        /// <param name="manipulators">manipulators</param>
        /// <returns></returns>
        [NotNull]
        public static Flex Flex(
            [NotNull] IEnumerable<IComponent> content,
            FlexDirection direction,
            params IManipulator[] manipulators
        ) => Li.Common.Layout.Flex.V(content, direction, manipulators);

        /// <summary>
        /// Creates flex row, see <see cref="Li.Common.Layout.Flex.V(IEnumerable{IComponent}, FlexDirection, IManipulator[])"/>
        /// </summary>
        /// <param name="content">content of the flex row</param>
        /// <param name="reverse">reverses content direction</param>
        /// <param name="manipulators">manipulators</param>
        /// <returns></returns>
        [NotNull]
        public static Flex Row(
            [NotNull] IEnumerable<IComponent> content,
            bool reverse = false,
            params IManipulator[] manipulators
        ) => Li.Common.Layout.Flex.V(content, reverse ? FlexDirection.RowReverse : FlexDirection.Row, (manipulators ?? Array.Empty<IManipulator>()).ToArray());
        
        /// <summary>
        /// Creates flex row, see <see cref="Li.Common.Layout.Flex.V(IEnumerable{IComponent}, FlexDirection, IManipulator[])"/>
        /// </summary>
        /// <param name="content">content of the flex row</param>
        /// <returns></returns>
        [NotNull]
        public static Flex Row(params IComponent[] content) =>
            Li.Common.Layout.Flex.V(content, FlexDirection.Row);
        
        /// <summary>
        /// Creates flex column, see <see cref="Li.Common.Layout.Flex.V(IEnumerable{IComponent}, FlexDirection, IManipulator[])"/>
        /// </summary>
        /// <param name="content">content of the flex column</param>
        /// <param name="reverse">reverses content direction</param>
        /// <param name="manipulators">manipulators</param>
        /// <returns></returns>
        [NotNull]
        public static Flex Col(
            [NotNull] IEnumerable<IComponent> content,
            bool reverse = false,
            IEnumerable<IManipulator> manipulators = null
        ) => Li.Common.Layout.Flex.V(content, reverse ? FlexDirection.ColumnReverse : FlexDirection.Column, (manipulators ?? Enumerable.Empty<IManipulator>()).ToArray());
        
        /// <summary>
        /// Creates flex column, see <see cref="Li.Common.Layout.Flex.V(IEnumerable{IComponent}, FlexDirection, IManipulator[])"/>
        /// </summary>
        /// <param name="content">content of the flex column</param>
        /// <returns></returns>
        [NotNull]
        public static Flex Col(params IComponent[] content) => Li.Common.Layout.Flex.V(content);
        
        /// <summary>
        /// Creates foldout component, see <see cref="Li.Common.Layout.Foldout.V(IComponent, IComponent, bool, bool, Li.Common.Layout.Foldout.HeaderContainer, Li.Common.Layout.Foldout.ContentContainer, Func{bool, Action, IComponent}, IManipulator[])"/>.
        /// </summary>
        /// <param name="header">header of the foldout</param>
        /// <param name="content">content of the foldout</param>
        /// <param name="initiallyOpen">should be open by default</param>
        /// <param name="nobToggleOnly">if true only toggle when clicking the nob, use whole header otherwise</param>
        /// <param name="headerContainer">container used to render header</param>
        /// <param name="contentContainer">container used to render content</param>
        /// <param name="manipulators">manipulators</param>
        /// <returns></returns>
        [NotNull]
        public static IComponent Foldout(
            [NotNull] IComponent header,
            [NotNull] IComponent content,
            bool initiallyOpen = false,
            bool nobToggleOnly = false,
            Foldout.HeaderContainer headerContainer = null,
            Foldout.ContentContainer contentContainer = null,
            params IManipulator[] manipulators
        ) => Li.Common.Layout.Foldout.V(header, content, initiallyOpen, nobToggleOnly, headerContainer, contentContainer, null, manipulators);
        
        /// <summary>
        /// Creates foldout component, see <see cref="Li.Common.Layout.Foldout.V(string, IComponent, bool, bool, Li.Common.Layout.Foldout.HeaderContainer, Li.Common.Layout.Foldout.ContentContainer, Func{bool, Action, IComponent}, IManipulator[])"/>.
        /// </summary>
        /// <param name="header">header text</param>
        /// <param name="content">content of the foldout</param>
        /// <param name="initiallyOpen">should be open by default</param>
        /// <param name="nobToggleOnly">if true only toggle when clicking the nob, us whole header otherwise</param>
        /// <param name="headerContainer">container used to render header</param>
        /// <param name="contentContainer">container used to render content</param>
        /// <param name="manipulators">manipulators</param>
        /// <returns></returns>
        [NotNull]
        public static IComponent Foldout(
            [NotNull] string header,
            [NotNull] IComponent content,
            bool initiallyOpen = false,
            bool nobToggleOnly = false,
            Foldout.HeaderContainer headerContainer = null,
            Foldout.ContentContainer contentContainer = null,
            params IManipulator[] manipulators
        ) => Li.Common.Layout.Foldout.V(header, content, initiallyOpen, nobToggleOnly, headerContainer, contentContainer, null, manipulators);
        
        /// <summary>
        /// Creates scrollable component, see <see cref="Li.Common.Layout.Scroll.V(IComponent, ScrollViewMode, Action{float, UI.Li.Common.Layout.Scroll.Orientation}, IManipulator[])"/>
        /// </summary>
        /// <param name="content">content of the scrollable</param>
        /// <param name="mode">mode of the scroll</param>
        /// <param name="onScroll">callback for scrolling</param>
        /// <param name="manipulators">manipulators</param>
        /// <returns></returns>
        [NotNull]
        public static Scroll Scroll(
            IComponent content,
            ScrollViewMode mode = ScrollViewMode.Vertical,
            Action<float, Scroll.Orientation> onScroll = null,
            params IManipulator[] manipulators
        ) => Li.Common.Layout.Scroll.V(content, mode, onScroll, manipulators);
        
        /// <summary>
        /// Creates split area component, see <see cref="Li.Common.Layout.SplitArea.V(IComponent, IComponent, TwoPaneSplitViewOrientation, float, bool, IManipulator[])"/>.
        /// </summary>
        /// <param name="mainContent">main area</param>
        /// <param name="secondaryContent">secondary area</param>
        /// <param name="orientation">orientation</param>
        /// <param name="initialSize">initial size of main area</param>
        /// <param name="reverse">places main area at the end of container</param>
        /// <param name="manipulators">manipulators</param>
        /// <returns></returns>
        [NotNull]
        public static SplitArea SplitArea(
            [NotNull] IComponent mainContent,
            [NotNull] IComponent secondaryContent,
            TwoPaneSplitViewOrientation orientation = TwoPaneSplitViewOrientation.Horizontal,
            float initialSize = 0,
            bool reverse = false,
            params IManipulator[] manipulators
        ) => Li.Common.Layout.SplitArea.V(mainContent, secondaryContent, orientation, initialSize, reverse, manipulators);

        /// <summary>
        /// Creates dropdown popup component, see <see cref="Li.Common.Layout.DropdownPopup.V"/>
        /// </summary>
        /// <param name="popupProvider">popup provider</param>
        /// <param name="popupContent">content of the popup</param>
        /// <param name="content">content to show popup around</param>
        /// <returns></returns>
        [NotNull]
        public static DropdownPopup DropdownPopup(
            [NotNull] DropdownPopup.Handler popupProvider,
            [NotNull] IComponent popupContent,
            [NotNull] IComponent content
        ) => Li.Common.Layout.DropdownPopup.V(content, popupProvider, popupContent);

        /// <summary>
        /// Creates dropdown popup provider for dropdown popup
        /// </summary>
        /// <returns></returns>
        [NotNull]
        public static DropdownPopup.Handler ProvideDropdown() => Li.Common.Layout.DropdownPopup.Provide();
    }
}