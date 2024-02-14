using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Li.Common.UIElements
{
    using Orientation = TwoPaneSplitViewOrientation;
    
    [PublicAPI]
    internal class TwoPaneSplitView : VisualElement
    {
        private const string ussClassName = "unity-two-pane-split-view";
        private const string contentContainerClassName = "unity-two-pane-split-view__content-container";
        private const string handleDragLineClassName = "unity-two-pane-split-view__dragline";
        private const string handleDragLineVerticalClassName = handleDragLineClassName + "--vertical";
        private const string handleDragLineHorizontalClassName = handleDragLineClassName + "--horizontal";
        private const string handleDragLineAnchorClassName = "unity-two-pane-split-view__dragline-anchor";

        private const string handleDragLineAnchorVerticalClassName = handleDragLineAnchorClassName + "--vertical";
        private const string handleDragLineAnchorHorizontalClassName = handleDragLineAnchorClassName + "--horizontal";

        private const string verticalClassName = "unity-two-pane-split-view--vertical";
        private const string horizontalClassName = "unity-two-pane-split-view--horizontal";

        public Orientation orientation
        {
            get => iOrientation;
            set
            {
                if (iOrientation == value)
                    return;
                
                Init(iFixedPaneIndex, iFixedPaneInitialDimension, value);
            }
        }

        public int fixedPaneIndex
        {
            get => iFixedPaneIndex;
            set
            {
                if (iFixedPaneIndex == value)
                    return;
                
                Init(value, iFixedPaneInitialDimension, orientation);
            }
        }

        public float fixedPaneInitialDimension
        {
            get => iFixedPaneInitialDimension;
            set
            {
                if (Math.Abs(iFixedPaneInitialDimension - value) < 0.01)
                    return;
                
                Init(iFixedPaneIndex, value, orientation);
            }
        }

        private VisualElement leftPane;
        private VisualElement rightPane;

        private VisualElement fixedPane;
        private VisualElement flexedPane;

        private readonly VisualElement dragLine;
        private readonly VisualElement dragLineAnchor;
        private float minDimension;

        private readonly VisualElement content;

        private Orientation iOrientation;
        private int iFixedPaneIndex;
        private float iFixedPaneInitialDimension;
        private float fixedPaneDimension;

        private SquareResizer resizer;

        public TwoPaneSplitView()
        {
            AddToClassList(ussClassName);

            content = new() { name = "unity-content-container" };
            content.AddToClassList(contentContainerClassName);
            hierarchy.Add(content);
            
            // Create drag anchor line.
            dragLineAnchor = new() { name = "unity-dragline-anchor" };
            dragLineAnchor.AddToClassList(handleDragLineAnchorClassName);
            hierarchy.Add(dragLineAnchor);

            // Create drag
            dragLine = new () { name = "unity-dragline" };
            dragLine.AddToClassList(handleDragLineClassName);
            dragLineAnchor.Add(dragLine);
        }

        public TwoPaneSplitView(
            int fixedPaneIndex,
            float fixedPaneStartDimension,
            Orientation orientation) : this()
        {
            Init(fixedPaneIndex, fixedPaneStartDimension, orientation);
        }

        public void UpdateChildren()
        {
            if (content.childCount != 2)
                return;

            leftPane = content[0];
            if (iFixedPaneIndex == 0)
            {
                fixedPane = leftPane;
                if (iOrientation == Orientation.Horizontal)
                    leftPane.style.width = fixedPaneDimension;
                else
                    leftPane.style.height = fixedPaneDimension;
            }
            else
            {
                flexedPane = leftPane;
            }
            
            rightPane = content[1];
            if (iFixedPaneIndex == 1)
            {
                fixedPane = rightPane;
                if (iOrientation == Orientation.Horizontal)
                    rightPane.style.width = fixedPaneDimension;
                else
                    rightPane.style.height = fixedPaneDimension;
            }
            else
            {
                flexedPane = rightPane;
            }

            fixedPane.style.flexShrink = 0;
            flexedPane.style.flexGrow = 1;
            flexedPane.style.flexShrink = 0;
            flexedPane.style.flexBasis = 0;

            if (iOrientation == Orientation.Horizontal)
            {
                if (iFixedPaneIndex == 0)
                    dragLineAnchor.style.left = fixedPaneDimension;
                else
                    dragLineAnchor.style.left = resolvedStyle.width - fixedPaneDimension;
            }
            else
            {
                if (iFixedPaneIndex == 0)
                    dragLineAnchor.style.top = fixedPaneDimension;
                else
                    dragLineAnchor.style.top = resolvedStyle.height - fixedPaneDimension;
            }
        }
        
        private void Init(int newFixedPaneIndex, float newFixedPaneInitialDimension, Orientation newOrientation)
        {
            iOrientation = newOrientation;
            minDimension = 100;
            iFixedPaneIndex = newFixedPaneIndex;
            iFixedPaneInitialDimension = newFixedPaneInitialDimension;
            fixedPaneDimension = iFixedPaneInitialDimension;

            if (iOrientation == Orientation.Horizontal)
                style.minWidth = iFixedPaneInitialDimension;
            else
                style.minHeight = iFixedPaneInitialDimension;

            content.RemoveFromClassList(horizontalClassName);
            content.RemoveFromClassList(verticalClassName);
            content.AddToClassList(iOrientation == Orientation.Horizontal ? horizontalClassName : verticalClassName);

            // Create drag anchor line.
            dragLineAnchor.RemoveFromClassList(handleDragLineAnchorHorizontalClassName);
            dragLineAnchor.RemoveFromClassList(handleDragLineAnchorVerticalClassName);
            dragLineAnchor.AddToClassList(iOrientation == Orientation.Horizontal
                ? handleDragLineAnchorHorizontalClassName
                : handleDragLineAnchorVerticalClassName);

            // Create drag
            dragLine.RemoveFromClassList(handleDragLineHorizontalClassName);
            dragLine.RemoveFromClassList(handleDragLineVerticalClassName);
            dragLine.AddToClassList(iOrientation == Orientation.Horizontal
                ? handleDragLineHorizontalClassName
                : handleDragLineVerticalClassName);

            if (resizer != null)
            {
                dragLineAnchor.RemoveManipulator(resizer);
                resizer = null;
            }

            UnregisterCallback<GeometryChangedEvent>(OnPostDisplaySetup);
            
            if (content.childCount != 2)
                RegisterCallback<GeometryChangedEvent>(OnPostDisplaySetup);
            else
                PostDisplaySetup();
        }

        private void OnPostDisplaySetup(GeometryChangedEvent evt)
        {
            if (content.childCount != 2)
                return;

            PostDisplaySetup();

            UnregisterCallback<GeometryChangedEvent>(OnPostDisplaySetup);
            
            UnregisterCallback<GeometryChangedEvent>(OnSizeChange);
            RegisterCallback<GeometryChangedEvent>(OnSizeChange);
        }

        private void PostDisplaySetup()
        {
            if (content.childCount != 2)
                return;

            UpdateChildren();

            int direction = iFixedPaneIndex == 0 ? 1 : -1;

            resizer = new(this, direction, minDimension, iOrientation);

            dragLineAnchor.AddManipulator(resizer);

            UnregisterCallback<GeometryChangedEvent>(OnPostDisplaySetup);
            UnregisterCallback<GeometryChangedEvent>(OnSizeChange);
            RegisterCallback<GeometryChangedEvent>(OnSizeChange);
        }

        void OnSizeChange(GeometryChangedEvent evt)
        {
            if (content.childCount != 2)
                return;
            
            var maxLength = resolvedStyle.width;
            var dragLinePos = dragLineAnchor.resolvedStyle.left;
            var activeElementPos = fixedPane.resolvedStyle.left;
            if (iOrientation == Orientation.Vertical)
            {
                maxLength = resolvedStyle.height;
                dragLinePos = dragLineAnchor.resolvedStyle.top;
                activeElementPos = fixedPane.resolvedStyle.top;
            }

            if (iFixedPaneIndex == 0 && dragLinePos > maxLength)
            {
                var delta = maxLength - dragLinePos;
                resizer.ApplyDelta(delta);
            }
            else if (iFixedPaneIndex == 1)
            {
                if (activeElementPos < 0)
                {
                    var delta = -dragLinePos;
                    resizer.ApplyDelta(delta);
                }
                else
                {
                    if (iOrientation == Orientation.Horizontal)
                        dragLineAnchor.style.left = activeElementPos;
                    else
                        dragLineAnchor.style.top = activeElementPos;
                }
            }
        }

        public override VisualElement contentContainer => content;

        private class SquareResizer : MouseManipulator
        {
            private Vector2 start;
            private bool active;
            private readonly TwoPaneSplitView splitView;
            private VisualElement pane => splitView.fixedPane;
            private readonly int direction;
            private readonly float minWidth;
            private readonly Orientation orientation;

            public SquareResizer(TwoPaneSplitView splitView, int dir, float minWidth, Orientation orientation)
            {
                this.orientation = orientation;
                this.minWidth = minWidth;
                this.splitView = splitView;
                direction = dir;
                activators.Add(new() { button = MouseButton.LeftMouse });
                active = false;
            }

            protected override void RegisterCallbacksOnTarget()
            {
                target.RegisterCallback<MouseDownEvent>(OnMouseDown);
                target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
                target.RegisterCallback<MouseUpEvent>(OnMouseUp);
            }

            protected override void UnregisterCallbacksFromTarget()
            {
                target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
                target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
                target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
            }

            public void ApplyDelta(float delta)
            {
                float oldDimension = orientation == Orientation.Horizontal
                    ? pane.resolvedStyle.width
                    : pane.resolvedStyle.height;
                float newDimension = oldDimension + delta;

                if (newDimension < oldDimension && newDimension < minWidth)
                    newDimension = minWidth;

                float maxLength = orientation == Orientation.Horizontal
                    ? splitView.resolvedStyle.width
                    : splitView.resolvedStyle.height;
                if (newDimension > oldDimension && newDimension > maxLength)
                    newDimension = maxLength;

                splitView.fixedPaneDimension = newDimension;
                
                if (orientation == Orientation.Horizontal)
                {
                    pane.style.width = newDimension;
                    if (splitView.iFixedPaneIndex == 0)
                        target.style.left = newDimension;
                    else
                        target.style.left = splitView.resolvedStyle.width - newDimension;
                }
                else
                {
                    pane.style.height = newDimension;
                    if (splitView.iFixedPaneIndex == 0)
                        target.style.top = newDimension;
                    else
                        target.style.top = splitView.resolvedStyle.height - newDimension;
                }
            }

            private void OnMouseDown(MouseDownEvent e)
            {
                if (active)
                {
                    e.StopImmediatePropagation();
                    return;
                }

                if (CanStartManipulation(e))
                {
                    start = e.localMousePosition;

                    active = true;
                    target.CaptureMouse();
                    e.StopPropagation();
                }
            }

            private void OnMouseMove(MouseMoveEvent e)
            {
                if (!active || !target.HasMouseCapture())
                    return;

                Vector2 diff = e.localMousePosition - start;
                float mouseDiff = diff.x;
                if (orientation == Orientation.Vertical)
                    mouseDiff = diff.y;

                float delta = direction * mouseDiff;

                ApplyDelta(delta);

                e.StopPropagation();
            }

            private void OnMouseUp(MouseUpEvent e)
            {
                if (!active || !target.HasMouseCapture() || !CanStopManipulation(e))
                    return;

                active = false;
                target.ReleaseMouse();
                e.StopPropagation();
            }
        }
    }
}