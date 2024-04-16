using JetBrains.Annotations;
using UI.Li.Common;
using UnityEngine.UIElements;

namespace UI.Li.Async
{
    [PublicAPI] public class ProgressBar: Element<UnityEngine.UIElements.ProgressBar>
    {
        private readonly float progress, min, max;
        
        [NotNull]
        public static ProgressBar V(float progress, float min = 0f, float max = 1f, params IManipulator[] manipulators) =>
            new(progress, min, max, manipulators);

        protected override UnityEngine.UIElements.ProgressBar PrepareElement(UnityEngine.UIElements.ProgressBar target)
        {
            var elem = base.PrepareElement(target);

            elem.lowValue = min;
            elem.highValue = max;
            elem.SetValueWithoutNotify(progress);
            
            return elem;
        }

        private ProgressBar(float progress, float min, float max, IManipulator[] manipulators): base(manipulators)
        {
            this.progress = progress;
            this.min = min;
            this.max = max;
        }
    }
}