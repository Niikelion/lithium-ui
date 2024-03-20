using System;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace UI.Li.lithium_ui.Runtime.Utils
{
    [PublicAPI] public class Portal : IComponent
    {
        [PublicAPI] public class Link
        {
            private VisualElement content, container;

            public VisualElement Content
            {
                set
                {
                    if (container != null && content != null) container.Remove(content);
                    content = value;
                    if (container != null && content != null) container.Add(content);
                }
            }
            
            public VisualElement Container
            {
                set
                {
                    if (container != null && content != null) container.Remove(content);
                    container = value;
                    if (container != null && content != null) container.Add(content);
                }
            }
        }

        [PublicAPI]
        public class Anchor : IComponent
        {
            public event Action<VisualElement> OnRender;

            [NotNull] private readonly Link link;
            private readonly VisualElement content = new();

            [NotNull]
            public static Anchor V([NotNull] Link link) => new(link);
            
            private Anchor([NotNull] Link link) => this.link = link;

            public void Dispose() { }

            public VisualElement Render()
            {
                link.Container = content;
                OnRender?.Invoke(content);
                return content;
            }

            public void Recompose(CompositionContext context)
            {
                context.StartFrame(this);
                context.EndFrame();
            }
        }
        
        public event Action<VisualElement> OnRender;

        [NotNull] private readonly Link link;
        [NotNull] private readonly IComponent content;
        private readonly VisualElement dummyContent = new()
        {
            style = { display = DisplayStyle.None }
        };

        [NotNull]
        public static Portal V([NotNull] IComponent content, [NotNull] Link link) =>
            new(content, link);
        
        private Portal([NotNull] IComponent content, [NotNull] Link link)
        {
            this.content = content;
            this.link = link;
        }

        public void Dispose() => content.Dispose();
        public VisualElement Render()
        {
            var element = content.Render();
            link.Content = element;
            OnRender?.Invoke(dummyContent);
            return dummyContent;
        }

        public void Recompose(CompositionContext context)
        {
            context.StartFrame(this);
            content.Recompose(context);
            context.EndFrame();
        }
    }
}