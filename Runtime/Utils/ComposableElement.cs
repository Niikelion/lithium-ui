using System;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace UI.Li.Utils
{
    /// <summary>
    /// <see cref="VisualElement"/> designed for easier use of compositions inside VisualElements systems.
    /// </summary>
    /// <seealso cref="CompositionRenderer"/>
    /// <remarks>Element may need manual disposing. See <see cref="SetContent"/> for more details.</remarks>
    [PublicAPI] public class ComposableElement: VisualElement, IDisposable
    {
        public bool NeedsDisposing { get; private set; }
        
        private CompositionRenderer renderer;

        /// <summary>
        /// Sets composition content for given element.
        /// </summary>
        /// <remarks>Disposed element will become empty and cannot be restored later. If you want to persist it during parent changes either dispose element manually or use <see cref="CompositionRenderer"/> directly.</remarks>
        /// <param name="composition">Composition to be used by element</param>
        /// <param name="disposeOnDetach">Indicates whether composition should be disposed off during <see cref="DetachFromPanelEvent"/> or not.</param>
        /// <exception cref="InvalidOperationException">Thrown when <c>SetContent</c> is called second time on the same element.</exception>
        [PublicAPI] public void SetContent([NotNull] IComposition composition, bool disposeOnDetach = false)
        {
            if (renderer != null)
                throw new InvalidOperationException("Cannot set content multiple times");

            NeedsDisposing = !disposeOnDetach;
            
            renderer = new CompositionRenderer(composition);
            
            if (!disposeOnDetach)
            {
                Add(renderer.UpdateAndRender());
                return;
            }

            RegisterCallback<AttachToPanelEvent>(OnAttachToParent);
            RegisterCallback<DetachFromPanelEvent>(OnDetachFromParent);
        }
        
        public void Dispose()
        {
            renderer?.Dispose();
            renderer = null;
            Clear();
        }
        
        private void OnAttachToParent(AttachToPanelEvent _) => Add(renderer?.UpdateAndRender());

        private void OnDetachFromParent(DetachFromPanelEvent _) => Dispose();

        ~ComposableElement()
        {
            Dispose();
        }
    }
}