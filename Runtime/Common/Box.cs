using System;
using JetBrains.Annotations;
using UnityEngine.UIElements;
using UI.Li.Utils.Continuations;

namespace UI.Li.Common
{
    /// <summary>
    /// Component used for wrapping single elements.
    /// </summary>
    [PublicAPI] public sealed class Box: Element
    {
        [CanBeNull] private readonly IComponent content;

        /// <summary>
        /// Creates <see cref="Box"/> instance with given content.
        /// </summary>
        /// <param name="content">content to be wrapped</param>
        /// <param name="data">additional element data <seealso cref="Element.Data"/></param>
        /// <returns></returns>
        [NotNull]
        [Obsolete]
        public static Box V(IComponent content, Data data) => new(content, data);

        /// <summary>
        /// Creates <see cref="Box"/> instance with given content.
        /// </summary>
        /// <param name="content">content to be wrapped</param>
        /// <param name="manipulators">manipulators <seealso cref="IManipulator"/></param>
        /// <returns></returns>
        [NotNull]
        public static Box V(IComponent content = null, params IManipulator[] manipulators) => new(content, manipulators);
        
        public override void Dispose()
        {
            content?.Dispose();
            base.Dispose();
        }

        public override bool StateLayoutEquals(IComponent other) => other is Box;

        protected override void OnState(CompositionContext context)
        {
            if (content == null) return;
            
            context.PreventNextEntryOverride();
            content.Recompose(context);
        }

        protected override VisualElement PrepareElement(VisualElement target)
        {
            var ret = base.PrepareElement(target);

            ret.Clear();
            
            content?.Run(c => ret.Add(c.Render()));
            
            return ret;
        }

        [Obsolete]
        private Box(IComponent content, Data data): base(data) => this.content = content;

        private Box(IComponent content, params IManipulator[] manipulators) : base(manipulators) => this.content = content;
    }
}