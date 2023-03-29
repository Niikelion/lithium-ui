using UnityEngine;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace UI.Li.Common
{
    public class SplitArea: Element
    {
        private readonly TwoPaneSplitViewOrientation orientation;
        private readonly float initialMainPanelSize;
        [NotNull] private readonly IComposition mainContent;
        [NotNull] private readonly IComposition secondaryContent;

        [NotNull]
        public static SplitArea V([NotNull] IComposition mainContent, [NotNull] IComposition secondaryContent,
            TwoPaneSplitViewOrientation orientation = TwoPaneSplitViewOrientation.Horizontal, float initialMainPanelSize = 0, Data data = new()) =>
            new(mainContent, secondaryContent, orientation, initialMainPanelSize, data);
        
        private SplitArea([NotNull] IComposition mainContent, [NotNull] IComposition secondaryContent,
            TwoPaneSplitViewOrientation orientation, float initialSize, Data data): base(data)
        {
            this.mainContent = mainContent;
            this.secondaryContent = secondaryContent;
            this.orientation = orientation;
            initialMainPanelSize = initialSize;
        }
        
        protected override VisualElement GetElement(VisualElement source)
        {
            var element = Use(source, () => new TwoPaneSplitView(0, initialMainPanelSize, orientation), p => p.orientation == orientation);

            element.contentContainer.Clear();

            return element;
        }

        protected override VisualElement PrepareElement(VisualElement target)
        {
            var element = base.PrepareElement(target) as TwoPaneSplitView;

            Debug.Assert(element != null);
            
            element.Add(mainContent.Render());
            element.Add(secondaryContent.Render());

            element.fixedPaneInitialDimension = initialMainPanelSize;

            return element;
        }
    }
}