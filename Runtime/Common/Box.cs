using JetBrains.Annotations;
using UnityEngine.UIElements;
using UI.Li.Utils.Continuations;

namespace UI.Li.Common
{
    /// <summary>
    /// Component used for wrapping single elements.
    /// </summary>
    public class Box: Element
    {
        [CanBeNull] private readonly IComponent content;

        /// <summary>
        /// Creates <see cref="Box"/> instance with given content.
        /// </summary>
        /// <param name="content">content to be wrapped</param>
        /// <param name="data">additional element data <seealso cref="Element.Data"/></param>
        /// <returns></returns>
        [PublicAPI]
        [NotNull]
        public static Box V(IComponent content = null, Data data = new()) => new(content, data);
        
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

        private Box(IComponent content, Data data): base(data)
        {
            this.content = content;
        }
    }
}