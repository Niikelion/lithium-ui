using System.Linq;
using JetBrains.Annotations;
using UnityEngine.UIElements;
using System.Collections.Generic;

namespace UI.Li.Common
{
    using RecompositionStrategy = CompositionContext.RecompositionStrategy;
    
    /// <summary>
    /// Composition representing standard flexbox container.
    /// </summary>
    /// <remarks>Content structure change between recompositions may result in state loss. If you cannot ensure constant number and ordering of child compositions consider using <see cref="List"/></remarks>
    /// <seealso cref="List"/>
    [PublicAPI] public class Flex: Element
    {
        [NotNull] private readonly IComposition[] content;
        private readonly FlexDirection direction;

        /// <summary>
        /// Constructs <see cref="Flex"/> instance.
        /// </summary>
        /// <param name="content">content of container</param>
        /// <param name="direction">direction of children</param>
        /// <param name="data">additional element data <seealso cref="Element.Data"/></param>
        /// <returns></returns>
        [PublicAPI] [NotNull] public static Flex V([NotNull] IEnumerable<IComposition> content, FlexDirection direction = FlexDirection.Column, Data data = new()) => new(direction, content, data);

        private Flex(FlexDirection direction, [NotNull] IEnumerable<IComposition> content, Data data): base(data)
        {
            this.direction = direction;
            this.content = content.ToArray();
        }

        protected override RecompositionStrategy RecompositionStrategy => RecompositionStrategy.Reorder;

        protected override void OnState(CompositionContext context)
        {
            foreach (var child in content)
                child.Recompose(context);
        }

        public override void Dispose()
        {
            foreach (var child in content)
                child.Dispose();
            
            base.Dispose();
        }

        protected override VisualElement PrepareElement(VisualElement target)
        {
            var ret = base.PrepareElement(target);

            ret.style.flexDirection = new StyleEnum<FlexDirection>(direction);

            ret.Clear();
            foreach (var child in content)
                ret.Add(child.Render());
            
            return ret;
        }
    }
}