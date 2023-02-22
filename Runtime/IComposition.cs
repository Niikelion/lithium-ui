using System;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace UI.Li
{
    /// <summary>
    /// Interface for composed UI elements.
    /// </summary>
    /// <remarks>Composition should only store constant creation parameters and temporary state used between recompositions and renders.</remarks>
    [PublicAPI] public interface IComposition: IDisposable
    {
        [PublicAPI] event Action<VisualElement> OnRender;

        /// <summary>
        /// Renders given composition. Consecutive returned values should be treated as new versions of the same composition.
        /// </summary>
        /// <returns>VisualElement representing given composition</returns>
        [PublicAPI] [NotNull] public VisualElement Render();

        /// <summary>
        /// <para>Recomposes given composition.</para>
        /// <para>
        /// To correctly store any data, including child compositions data call <see cref="CompositionContext.StartFrame"/> at the start of the method and <see cref="CompositionContext.EndFrame"/> at the end.
        /// Then you can use <see cref="CompositionContext.Use{T}(T)"/>, <see cref="CompositionContext.Remember{T}"/>, <see cref="CompositionContext.RememberF{T}"/> or recompose children, but remember that you can't declare more state variables after first child starts recomposing.
        /// </para>
        /// </summary>
        /// <remarks>It is used for every recomposition, including initial composition.</remarks>
        /// <param name="context">context containing state data.</param>
        [PublicAPI] public void Recompose([NotNull] CompositionContext context);
    }
}