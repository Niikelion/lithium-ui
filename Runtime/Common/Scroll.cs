using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace UI.Li.Common
{
    /// <summary>
    /// Composition representing <see cref="UnityEngine.UIElements.ScrollView"/>
    /// </summary>
    public class Scroll: Element
    {
        private readonly IComponent content;
        private readonly ScrollViewMode mode;

        [PublicAPI]
        [NotNull]
        public static Scroll V([NotNull] IComponent content, ScrollViewMode mode = ScrollViewMode.Vertical,
            Data data = new()) =>
            new(content, mode, data);

        private Scroll([NotNull] IComponent content, ScrollViewMode mode, Data data): base(data)
        {
            this.content = content;
            this.mode = mode;
        }
        
        protected override VisualElement PrepareElement(VisualElement target)
        {
            var ret = base.PrepareElement(target);

            ret.AddToClassList("unity-scroll-view");

            return ret;
        }
        
        protected override VisualElement GetElement(VisualElement source)
        {
            var element = Use<ScrollView>(source);
            element.contentContainer.Clear();

            element.horizontalScroller.valueChanged += OnVerticalScrollChanged;

            return element;
        }

        private void OnVerticalScrollChanged(float value)
        {
            //
        }
    }
}