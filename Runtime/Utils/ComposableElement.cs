using System;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace UI.Li.Utils
{
    /// <summary>
    /// <see cref="VisualElement"/> designed for easier use of components inside UI Elements.
    /// </summary>
    /// <seealso cref="ComponentRenderer"/>
    /// <remarks>Element may need manual disposing. See <see cref="SetContent"/> for more details.</remarks>
    [PublicAPI] public class ComposableElement: VisualElement, IDisposable
    {
        public bool NeedsDisposing { get; private set; }
        
        private ComponentRenderer renderer;

        /// <summary>
        /// Sets component content for given element.
        /// </summary>
        /// <remarks>Disposed element will become empty and cannot be restored later. If you want to persist it during parent changes either dispose element manually or use <see cref="ComponentRenderer"/> directly.</remarks>
        /// <param name="component">Component to be used by element</param>
        /// <param name="disposeOnDetach">Indicates whether component should be disposed off during <see cref="DetachFromPanelEvent"/> or not.</param>
        /// <exception cref="InvalidOperationException">Thrown when <c>SetContent</c> is called second time on the same element.</exception>
        public void SetContent([NotNull] IComponent component, bool disposeOnDetach = false) => SetContent(() => component, disposeOnDetach);

        public void SetContent([NotNull] Func<IComponent> componentFactory, bool disposeOnDetach = false)
        {
            if (renderer != null)
                throw new InvalidOperationException("Cannot set content multiple times");

            NeedsDisposing = !disposeOnDetach;
            
            renderer = new(componentFactory);
            
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