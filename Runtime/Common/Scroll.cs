using System;
using JetBrains.Annotations;
using UI.Li.Utils.Continuations;
using UnityEngine.UIElements;

namespace UI.Li.Common
{
    /// <summary>
    /// Composition representing <see cref="UnityEngine.UIElements.ScrollView"/>
    /// </summary>
    public class Scroll: Element
    {
        public enum Orientation
        {
            Vertical,
            Horizontal
        }
        
        private readonly IComponent content;
        private readonly ScrollViewMode mode;
        private readonly Action<float, Orientation> onScroll;

        [PublicAPI]
        [NotNull]
        public static Scroll V([NotNull] IComponent content, ScrollViewMode mode = ScrollViewMode.Vertical,
            Action<float, Orientation> onScroll = null, Data data = new()) =>
            new(content, mode, onScroll, data);

        protected override void OnState(CompositionContext context)
        {
            context.PreventNextEntryOverride();
            content?.Recompose(context);
        }

        public override void Dispose()
        {
            content?.Dispose();
            base.Dispose();
        }

        protected override VisualElement GetElement(VisualElement source)
        {
            var element = Use<ScrollView>(source, true);

            element.mode = mode;
            element.verticalScroller.valueChanged += OnVerticalScrollChanged;
            element.horizontalScroller.valueChanged += OnHorizontalScrollChanged;
            
            AddCleanup(element, () =>
            {
                element.verticalScroller.valueChanged -= OnVerticalScrollChanged;
                element.horizontalScroller.valueChanged -= OnHorizontalScrollChanged;
            });

            return element;
        }
        
        protected override VisualElement PrepareElement(VisualElement target)
        {
            var ret = base.PrepareElement(target);

            ret.AddToClassList(ScrollView.ussClassName);

            content?.Run(c => ret.Add(c.Render()));
            
            return ret;
        }

        //TODO: add scroll handlers
        private void OnVerticalScrollChanged(float value) => onScroll?.Invoke(value, Orientation.Vertical);

        private void OnHorizontalScrollChanged(float value) => onScroll?.Invoke(value, Orientation.Horizontal);
        
        private Scroll([NotNull] IComponent content, ScrollViewMode mode, Action<float, Orientation> onScroll, Data data): base(data)
        {
            this.content = content;
            this.mode = mode;
            this.onScroll = onScroll;
        }
    }
}