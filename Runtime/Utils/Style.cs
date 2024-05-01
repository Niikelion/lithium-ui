using JetBrains.Annotations;
using UI.Li.Common;
using UnityEngine.UIElements;

namespace UI.Li.Utils
{
    public sealed class StyleWrapper: Wrapper //TODO: add option to combine Wrappers to reduce instance count(possibly, by copying StyleWrapper properties when wrapping StyleWrapper with StyleWrapper)
    {
        private readonly Style style;
        private  readonly bool empty;
        
        [NotNull]
        public static StyleWrapper V([System.Diagnostics.CodeAnalysis.NotNull] IComponent component, Style style) => new (component, style);
        
        public StyleWrapper(IComponent component, Style style) : base(component)
        {
            this.style = style;
            empty = false;
        }

        public StyleWrapper(IComponent component) : base(component)
        {
            style = new();
            empty = true;
        }

        public override VisualElement Render()
        {
            var ret =  base.Render();

            if (empty)
                return ret;
            
            var previous = Style.CopyFromElement(ret);
            
            style.ApplyToElement(ret);
            
            CompositionContext.ElementUserData.AppendCleanupAction(ret, () =>
            {
                previous.ApplyToElement(ret);
            });
            
            return ret;
        }
    }
}