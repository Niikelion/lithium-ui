using System;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace UI.Li
{
    /// <summary>
    /// Class designed for creating compositions using other compositions.
    /// </summary>
    [PublicAPI] public class Composition: IComposition
    {
        public event Action<VisualElement> OnRender;
        public event Action<CompositionContext> OnBeforeRecompose;
        
        [NotNull] private readonly Func<CompositionContext, IComposition> composer;
        private readonly bool isStatic;
        private IComposition innerComposition;

        /// <summary>
        /// Creates Composition using given function.
        /// </summary>
        /// <param name="composer">composer function used to create composition.</param>
        /// <param name="isStatic">indicates whether or not structure of given composition will change over time(some data or parameters can change, just not composition structure).</param>
        /// <remarks><see cref="isStatic"/> only constraints returned element and not its children. For example, if you declare your composition static and on first render you return <see cref="Common.Text"/>, you must return some <see cref="Common.Text"/> every time.</remarks>
        [PublicAPI] public Composition([NotNull] Func<CompositionContext, IComposition> composer, bool isStatic = false)
        {
            this.composer = composer;
            this.isStatic = isStatic;
        }

        public VisualElement Render()
        {
            var ret = innerComposition?.Render() ?? throw new InvalidOperationException("Rendering composition before applying context");

            OnRender?.Invoke(ret);
            
            return ret;
        }

        public void Recompose(CompositionContext context)
        {
            OnBeforeRecompose?.Invoke(context);
            context.StartFrame(this);
            innerComposition = composer(context);
            if (isStatic)
                context.PreventNextEntryOverride();
            innerComposition.Recompose(context);
            context.EndFrame();
        } 

        public void Dispose()
        {
            innerComposition?.Dispose();
            OnRender = null;
        }
    }
}