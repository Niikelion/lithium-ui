using System.Linq;
using JetBrains.Annotations;
using UnityEngine.UIElements;
using System.Collections.Generic;

namespace UI.Li.Common.Layout
{
    using RecompositionStrategy = CompositionContext.RecompositionStrategy;
    
    /// <summary>
    /// Component representing standard flexbox container.
    /// </summary>
    /// <remarks>Content structure change between recompositions may result in state loss. If you cannot ensure constant number and ordering of child elements consider using <see cref="List"/></remarks>
    /// <seealso cref="List"/>
    [PublicAPI] public sealed class Flex: Element
    {
        [NotNull] private readonly IComponent[] content;
        private readonly FlexDirection direction;

        /// <summary>
        /// Constructs <see cref="Flex"/> instance.
        /// </summary>
        /// <param name="content">content of container</param>
        /// <param name="direction">direction of children</param>
        /// <param name="manipulators">manipulators <seealso cref="IManipulator"/></param>
        /// <returns></returns>
        [NotNull] public static Flex V([NotNull] IEnumerable<IComponent> content, FlexDirection direction = FlexDirection.Column, params IManipulator[] manipulators) => new(direction, content, manipulators);
        
        public override void Dispose()
        {
            foreach (var child in content)
                child.Dispose();
            
            base.Dispose();
        }

        public override bool StateLayoutEquals(IComponent other) =>
            other is Flex flex && content.Length == flex.content.Length;

        protected override RecompositionStrategy RecompositionStrategy => RecompositionStrategy.Reorder;

        protected override void OnState(CompositionContext context)
        {
            foreach (var child in content)
                child.Recompose(context);
        }

        protected override VisualElement PrepareElement(VisualElement target)
        {
            var ret = base.PrepareElement(target);

            ret.style.flexDirection = direction;
            ret.Clear();
            
            foreach (var child in content)
            {
                var childElement = child.Render();
                if (childElement == null)
                    continue;
                
                ret.Add(childElement);
            }
            
            return ret;
        }
        
        private Flex(FlexDirection direction, [NotNull] IEnumerable<IComponent> content, IManipulator[] manipulators): base(manipulators)
        {
            this.direction = direction;
            this.content = content.ToArray();
        }
    }
}