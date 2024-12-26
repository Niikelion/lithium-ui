using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace UI.Li.Utils
{
    public class ManipulatorWrapper: Wrapper
    {
        private readonly IManipulator[] manipulators;
        
        public ManipulatorWrapper(IComponent innerComponent, params IManipulator[] manipulators) : base(innerComponent) =>
            this.manipulators = manipulators;

        public override VisualElement Render()
        {
            var ret = base.Render();

            CompositionContext.ElementUserData.AddManipulators(ret, manipulators);
            
            return ret;
        }
    }

    [PublicAPI] public static class ManipulatorExtensions
    {
        public static IComponent Manipulate(this IComponent component, params IManipulator[] manipulators) =>
            new ManipulatorWrapper(component, manipulators);
    }
    
    [PublicAPI] public class Tooltipped: Manipulator
    {
        private readonly string tooltip;
        private string previousTooltip;

        public Tooltipped(string tooltip) => this.tooltip = tooltip;
        
        protected override void RegisterCallbacksOnTarget()
        {
            previousTooltip = target.tooltip;
            target.tooltip = tooltip;
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.tooltip = previousTooltip;
        }
    }

    [PublicAPI]
    public class Disabled : Manipulator
    {
        private bool previousEnabled;

        protected override void RegisterCallbacksOnTarget()
        {
            previousEnabled = target.enabledSelf;
            target.SetEnabled(false);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.SetEnabled(previousEnabled);
        }
    }
}