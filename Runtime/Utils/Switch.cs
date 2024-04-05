using System;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace UI.Li.Utils
{
    public class SwitchWrapper : IComponent
    {
        private readonly IComponent content;
        private readonly int choice;

        [NotNull]
        public static SwitchWrapper V(IComponent content, int choice) => new(content, choice);

        public void Dispose() => content?.Dispose();

        public event Action<VisualElement> OnRender;

        public VisualElement Render()
        {
            var element = content?.Render() ?? new VisualElement();

            OnRender?.Invoke(element);

            return element;
        }

        public void Recompose(CompositionContext context)
        {
            context.StartFrame(this);

            if (content != null)
            {
                context.SetNextEntryId(choice + 1);
                content.Recompose(context);
            }

            context.EndFrame();
        }

        public override string ToString() => $"Switch[{choice}]";

        public bool StateLayoutEquals(IComponent other)
        {
            if (other is not SwitchWrapper wrapper)
                return false;

            if (choice != wrapper.choice)
                return false;

            if (content == null || wrapper.content == null)
                return content == wrapper.content;

            return content.StateLayoutEquals(wrapper.content);
        }

        private SwitchWrapper(IComponent content, int choice)
        {
            this.content = content;
            this.choice = choice;
        }
    }
}