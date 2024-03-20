using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine.UIElements;
using System.Collections.Generic;

namespace UI.Li.Common
{
    using RecompositionStrategy = CompositionContext.RecompositionStrategy;
    
    /// <summary>
    /// Component representing standard flexbox container.
    /// </summary>
    /// <remarks>Content structure change between recompositions may result in state loss. If you cannot ensure constant number and ordering of child elements consider using <see cref="List"/></remarks>
    /// <seealso cref="List"/>
    [PublicAPI] public class Flex: Element
    {
        [NotNull] private readonly IComponent[] content;
        private readonly FlexDirection direction;

        /// <summary>
        /// Constructs <see cref="Flex"/> instance.
        /// </summary>
        /// <param name="content">content of container</param>
        /// <param name="direction">direction of children</param>
        /// <param name="data">additional element data <seealso cref="Element.Data"/></param>
        /// <returns></returns>
        [NotNull] [Obsolete] public static Flex V([NotNull] IEnumerable<IComponent> content, FlexDirection direction, Data data) => new(direction, content, data);

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

        [Obsolete] private Flex(FlexDirection direction, [NotNull] IEnumerable<IComponent> content, Data data): base(data)
        {
            this.direction = direction;
            this.content = content.ToArray();
        }
        
        private Flex(FlexDirection direction, [NotNull] IEnumerable<IComponent> content, IManipulator[] manipulators): base(manipulators)
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

        protected override VisualElement PrepareElement(VisualElement target)
        {
            var ret = base.PrepareElement(target);

            ret.style.flexDirection = direction;

            foreach (var child in content)
                ret.Add(child.Render());
            
            return ret;
        }
    }
}