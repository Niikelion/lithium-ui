using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace UI.Li.Utils
{
    // TODO: make proper documentation instead of memo for the expected functionality
    /// <summary>
    /// Component responsible for caching parts of ui.
    /// It should be able to prevent recompose and rerender cascades based on provided variables by
    /// comparing their values.
    /// Right now it can't be done since there is no way of skipping over state without reading or clearing it.
    /// When done, it could be used for optimization, by cutting cascade of object allocations.
    /// </summary>
    public class MemoWrapper: IComponent
    {
        [NotNull] private readonly object[] vars;
        private readonly IComponent innerComponent;
        
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public event Action<VisualElement> OnRender;
        public VisualElement Render()
        {
            throw new NotImplementedException();
        }

        public void Recompose(CompositionContext context)
        {
            context.SetNextEntryOrigin(this);
            
            var oldVars = context.RememberRef(vars);

            innerComponent.Recompose(context);
            
            if (oldVars.Value.Length != vars.Length ||
                oldVars.Value.Zip(vars, Equals).Any(v => !v))
            {
                oldVars.Value = vars;
                // TODO: full recompose and return
            }
            
            // TODO: preserve frames
        }

        public bool StateLayoutEquals(IComponent other) =>
            other is MemoWrapper memo && innerComponent.StateLayoutEquals(memo.innerComponent);

        private MemoWrapper([NotNull] object[] vars) => this.vars = vars;
    }
}