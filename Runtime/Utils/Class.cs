using System.Linq;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace UI.Li.Utils
{
    [PublicAPI]
    public class ClassWrapper: Wrapper
    {
        private readonly string[] classes;
        
        public ClassWrapper(IComponent innerComponent, params string[] classes) : base(innerComponent) => this.classes = classes;

        public override VisualElement Render()
        {
            var ret = base.Render();

            var prev = ret.GetClasses().ToHashSet();
            
            CompositionContext.ElementUserData.AppendCleanupAction(ret, () =>
            {
                foreach (var cls in classes)
                    if (!prev.Contains(cls))
                        ret.RemoveFromClassList(cls);
            });
            
            foreach (var cls in classes)
                ret.AddToClassList(cls);
            
            return ret;
        }
    }
}