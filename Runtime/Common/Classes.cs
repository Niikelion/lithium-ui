using System.Collections.Generic;
using JetBrains.Annotations;
using UI.Li.Utils;
using UnityEngine.UIElements;

namespace UI.Li.lithium_ui.Runtime.Common
{
    public sealed class ClassNameWrapper: Wrapper
    {
        private readonly string[] classNames;
        
        public ClassNameWrapper(IComponent innerComponent, params string[] classNames) : base(innerComponent) =>
            this.classNames = classNames;

        public override VisualElement Render()
        {
            var element = base.Render();

            var addedClasses = new List<string>();

            foreach (var className in classNames)
            {
                if (element.ClassListContains(className))
                    continue;
                element.AddToClassList(className);
                addedClasses.Add(className);
            }

            CompositionContext.ElementUserData.AppendCleanupAction(element, () =>
            {
                foreach (var className in addedClasses)
                    element.RemoveFromClassList(className);
            });
            
            return element;
        }
    }

    public sealed class StyleSheetWrapper : Wrapper
    {
        private readonly StyleSheet[] styleSheets;
        
        public StyleSheetWrapper(IComponent innerComponent, params StyleSheet[] styleSheets) : base(innerComponent) =>
            this.styleSheets = styleSheets;

        public override VisualElement Render()
        {
            var element = base.Render();

            var addedStyleSheets = new List<StyleSheet>();
            
            foreach (var styleSheet in styleSheets) {
                if (element.styleSheets.Contains(styleSheet))
                    continue;
                
                element.styleSheets.Add(styleSheet);
                addedStyleSheets.Add(styleSheet);
            }
            
            CompositionContext.ElementUserData.AppendCleanupAction(element, () =>
            {
                foreach (var styleSheet in addedStyleSheets)
                {
                    if (!styleSheet)
                        continue;
                    
                    element.styleSheets.Remove(styleSheet);
                }
            });
            
            return element;
        }
    }

    [PublicAPI]
    public static class ClassNameExtensions
    {
        public static IComponent WithClasses(this IComponent component, params string[] classNames) =>
            new ClassNameWrapper(component, classNames);

        public static IComponent WithStyleSheets(this IComponent component, params StyleSheet[] styleSheets) =>
            new StyleSheetWrapper(component, styleSheets);
    }
}