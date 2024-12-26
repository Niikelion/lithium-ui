using UnityEngine;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace UI.Li.Common.Layout
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
        /// <param name="manipulators">manipulators <seealso cref="IManipulator"/></param>
        /// <returns></returns>
        [NotNull]
        public static SplitArea V([NotNull] IComponent mainContent, [NotNull] IComponent secondaryContent,
            TwoPaneSplitViewOrientation orientation = TwoPaneSplitViewOrientation.Horizontal, float initialMainPanelSize = 100, bool reverse = false, params IManipulator[] manipulators) =>
            new(mainContent, secondaryContent, orientation, initialMainPanelSize, reverse, manipulators);
        
        private SplitArea([NotNull] IComponent mainContent, [NotNull] IComponent secondaryContent,
            TwoPaneSplitViewOrientation orientation, float initialSize, bool reverse, IManipulator[] manipulators): base(manipulators)
        {
            this.mainContent = mainContent;
            this.secondaryContent = secondaryContent;
            this.orientation = orientation;
            this.reverse = reverse;
            initialMainPanelSize = initialSize;
        }
        
        protected override VisualElement GetElement(VisualElement source) =>
            Use(source, () => new UIElements.TwoPaneSplitView(reverse ? 1 : 0, initialMainPanelSize, orientation), _ => true);

        protected override void OnState(CompositionContext context)
        {
            base.OnState(context);
            context.PreventNextEntryOverride();
            mainContent.Recompose(context);
            context.PreventNextEntryOverride();
            secondaryContent.Recompose(context);
        }

        protected override VisualElement PrepareElement(VisualElement target)
        {
            var element = base.PrepareElement(target) as UI.Li.Common.UIElements.TwoPaneSplitView;
            
            Debug.Assert(element != null);

            element.AddToClassList(UIElements.TwoPaneSplitView.ussClassName);

            element.fixedPaneInitialDimension = initialMainPanelSize;
            element.fixedPaneIndex = reverse ? 1 : 0;
            element.orientation = orientation;
            
            element.flexedElement = mainContent.Render();
            element.fixedElement = secondaryContent.Render();
            
            return element;
        }
    }
}