using System;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace UI.Li.Utils
{
    [PublicAPI] public sealed class Portal : IComponent
    {
        [PublicAPI] public class Link
        {
            private VisualElement content, container;

            public Link() { }

            public Link(VisualElement container) => Container = container;

            public VisualElement Content
            {
                set
                {
                    if (content == value)
                        return;
                    
                    if (container != null && content != null && content.parent == container) container.Remove(content);
                    content = value;
                    if (container != null && content != null) container.Add(content);
                }
            }
            
            public VisualElement Container
            {
                set
                {
                    if (container == value)
                        return;
                    
                    if (container != null && content != null && content.parent == container) container.Remove(content);
                    container = value;
                    if (container != null && content != null) container.Add(content);
                }
            }
        }

        [PublicAPI]
        public class Anchor : IComponent
        {
            private event Action<VisualElement> OnRender;

            event Action<VisualElement> IComponent.OnRender
            {
                add => OnRender += value;
                remove => OnRender -= value;
            }

            public IComponent UnderlyingComponent => this;

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

            public bool StateLayoutEquals(IComponent other) => other is Anchor;
        }

        private event Action<VisualElement> OnRender;

        event Action<VisualElement> IComponent.OnRender
        {
            add => OnRender += value;
            remove => OnRender -= value;
        }

        public IComponent UnderlyingComponent => this;
        
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

        public bool StateLayoutEquals(IComponent other) => other is Portal;
    }
}