using UnityEngine;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace UI.Li.Common
{
    /// <summary>
    /// Component representing <see cref="TwoPaneSplitView"/>.
    /// </summary>
    public class SplitArea: Element
    {
        private readonly TwoPaneSplitViewOrientation orientation;
        private readonly float initialMainPanelSize;
        private readonly bool reverse;
        [NotNull] private readonly IComponent mainContent;
        [NotNull] private readonly IComponent secondaryContent;

        /// <summary>
        /// Creates <see cref="SplitArea"/> instance with given content.
        /// </summary>
        /// <param name="mainContent">main content</param>
        /// <param name="secondaryContent">secondary content</param>
        /// <param name="orientation">orientation of container</param>
        /// <param name="initialMainPanelSize">initial main panel size</param>
        /// <param name="reverse">places main content at the end of the container, instead of start</param>
        /// <param name="data">additional element data <seealso cref="Element.Data"/></param>
        /// <returns></returns>
        [PublicAPI]
        [NotNull]
        public static SplitArea V([NotNull] IComponent mainContent, [NotNull] IComponent secondaryContent,
            TwoPaneSplitViewOrientation orientation = TwoPaneSplitViewOrientation.Horizontal, float initialMainPanelSize = 100, bool reverse = false, Data data = new()) =>
            new(mainContent, secondaryContent, orientation, initialMainPanelSize, reverse, data);
        
        private SplitArea([NotNull] IComponent mainContent, [NotNull] IComponent secondaryContent,
            TwoPaneSplitViewOrientation orientation, float initialSize, bool reverse, Data data): base(data)
        {
            this.mainContent = mainContent;
            this.secondaryContent = secondaryContent;
            this.orientation = orientation;
            this.reverse = reverse;
            initialMainPanelSize = initialSize;
        }
        
        protected override VisualElement GetElement(VisualElement source)
        {
            var element = Use(source, () => new TwoPaneSplitView(reverse ? 1 : 0, initialMainPanelSize, orientation), p => p.orientation == orientation && p.fixedPaneIndex == (reverse ? 1 : 0));

            element.contentContainer.Clear();

            return element;
        }

        protected override VisualElement PrepareElement(VisualElement target)
        {
            var element = base.PrepareElement(target) as TwoPaneSplitView;

            Debug.Assert(element != null);

            if (!reverse)
            {
                element.Add(mainContent.Render());
                element.Add(secondaryContent.Render());
            }
            else
            {
                element.Add(secondaryContent.Render());
                element.Add(mainContent.Render());
            }

            element.fixedPaneInitialDimension = initialMainPanelSize;

            return element;
        }
    }
}