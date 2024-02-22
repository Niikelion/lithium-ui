using System;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace UI.Li
{
    /// <summary>
    /// Interface for composed UI elements.
    /// </summary>
    /// <remarks>Component should only store constant creation parameters and temporary state used between recompositions and renders.</remarks>
    [PublicAPI] public interface IComponent: IDisposable
    {
        event Action<VisualElement> OnRender;

        /// <summary>
        /// Renders given component. Consecutive returned values should be treated as new versions of the same component.
        /// </summary>
        /// <returns>VisualElement representing given component</returns>
        [NotNull] public VisualElement Render();

        /// <summary>
        /// <para>Recomposes given component.</para>
        /// <para>
        /// To correctly store any data, including child compositions data call <see cref="CompositionContext.StartFrame"/> at the start of the method and <see cref="CompositionContext.EndFrame"/> at the end.
        /// Then you can use <see cref="CompositionContext.Use{T}(T)"/>, <see cref="CompositionContext.Remember{T}"/>, <see cref="CompositionContext.RememberF{T}"/> or recompose children, but remember that you can't declare more state variables after first child starts recomposing.
        /// </para>
        /// </summary>
        /// <remarks>It is used for every recomposition, including initial component.</remarks>
        /// <param name="context">context containing state data.</param>
        public void Recompose([NotNull] CompositionContext context);
    }
}