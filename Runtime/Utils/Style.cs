using JetBrains.Annotations;
using UI.Li.Common;
using UnityEngine.UIElements;

namespace UI.Li.Utils
{
    public sealed class StyleWrapper: Wrapper
    {
        private readonly Style style;
        private  readonly bool empty;
        
        [NotNull]
        public static StyleWrapper V([System.Diagnostics.CodeAnalysis.NotNull] IComponent component, Style style) => new (component, style);
        
        public StyleWrapper(IComponent component, Style style) : base(ExtractComponent(component))
        {
            this.style = component is StyleWrapper { empty: false } wrapper ? style.Extend(wrapper.style) : style;
            empty = false;
        }

        public StyleWrapper(IComponent component) : base(ExtractComponent(component))
        {
            if (component is not StyleWrapper wrapper)
            {
                style = new();
                empty = true;
                return;
            }

            style = wrapper.style;
            empty = wrapper.empty;
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

        private static IComponent ExtractComponent(IComponent component) =>
            component is StyleWrapper wrapper ? wrapper.InnerComponent : component;
    }
}