using System;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace UI.Li.Utils
{
    /// <summary>
    /// Lightweight static composition class that uses provided function to render composition.
    /// </summary>
    /// <remarks>Can be used to easily wrap VisualElements</remarks>
    public class LeafComposition: IComposition
    {
        public event Action<VisualElement> OnRender;
        public event Action<CompositionContext> OnBeforeRecompose;
        
        [NotNull] private readonly Func<VisualElement, VisualElement> renderer;
        private readonly bool usesCache;
        private VisualElement previouslyRendered;

        /// <summary>
        /// Creates composition using renderer function.
        /// </summary>
        [PublicAPI] public LeafComposition([NotNull] Func<VisualElement, VisualElement> renderer)
        {
            this.renderer = renderer;
            usesCache = true;
        }

        /// <summary>
        /// Creates composition using renderer function.
        /// </summary>
        [PublicAPI] public LeafComposition([NotNull] Func<VisualElement> renderer) : this(_ => renderer()) { }

        public VisualElement Render()
        {
            var ret = renderer(previouslyRendered);
            
            OnRender?.Invoke(ret);
            
            return ret;
        }

        public void Recompose(CompositionContext context)
        {
            OnBeforeRecompose?.Invoke(context);
            
            if (!usesCache)
            {
                context.SetNextEntryId();
                return;
            }

            previouslyRendered = context.StartFrame(this);
            context.EndFrame();
        }
        
        public void Dispose()
        {
            previouslyRendered = null;
            OnRender = null;
        }
    }
}