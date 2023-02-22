using JetBrains.Annotations;
using UnityEngine.UIElements;
using UI.Li.Utils.Continuations;

namespace UI.Li.Common
{
    public class Box: Element
    {
        [CanBeNull] private readonly IComposition content;

        [PublicAPI]
        [NotNull]
        public static Box V(IComposition content = null, Data data = new()) => new(content, data);
        
        public override void Dispose()
        {
            content?.Dispose();
            base.Dispose();
        }

        protected override void OnState(CompositionContext context) => content?.Recompose(context);

        protected override VisualElement PrepareElement(VisualElement target)
        {
            var ret = base.PrepareElement(target);

            ret.Clear();
            
            content?.Run(c => ret.Add(c.Render()));
            
            return ret;
        }

        private Box(IComposition content, Data data): base(data)
        {
            this.content = content;
        }
    }
}