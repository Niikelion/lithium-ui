using System;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace UI.Li.Common.Layout
{
    /// <summary>
    /// Base component for modals, dropdowns and other popups
    /// </summary>
    [PublicAPI]
    public abstract class Popup: IComponent
    {
        /// <summary>
        /// Base class for popup handler that acts as a popup controller.
        /// </summary>
        [PublicAPI]
        public abstract class PopupHandler
        {
            protected UIElements.Popup PopupInstance;
            
            /// <summary>
            /// Closes the popup.
            /// </summary>
            public void Close() => PopupInstance?.Close();
            
            internal void ProvidePopup(UIElements.Popup popup)
            {
                if (popup == PopupInstance)
                    return;

                PopupInstance?.Close();
                PopupInstance = popup;
            }
        }
        
        private event Action<VisualElement> OnRender;
        [NotNull] private readonly IComponent content;
        [NotNull] private readonly IComponent popupContent;
        private MutableValue<UIElements.Popup> popup;
        private PopupHandler handler;

        protected UIElements.Popup PopupInstance => popup.Value;
        
        public IComponent UnderlyingComponent => this;

        event Action<VisualElement> IComponent.OnRender
        {
            add => OnRender += value;
            remove => OnRender -= value;
        }

        protected Popup([NotNull] IComponent content, PopupHandler handler, [NotNull] IComponent popupContent)
        {
            this.content = content;
            this.popupContent = popupContent;
            this.handler = handler;
        }
        
        public void Dispose()
        {
            content.Dispose();
            popupContent.Dispose();
        }

        public virtual VisualElement Render()
        {
            var element = content.Render();
            var popupElement = popupContent.Render();

            var popupContainer = PopupInstance.Container;

            if (popupContainer.childCount > 0)
            {
                if (popupContainer[0] == popupElement)
                {
                    OnRender?.Invoke(element);
                    return element;
                }
                
                popupContainer.Clear();
            }

            popupContainer.Add(popupElement);
            
            OnRender?.Invoke(element);
            
            return element;
        }

        public void Recompose(CompositionContext context)
        {
            context.StartFrame(this);

            popup = context.RememberF(() => new UIElements.Popup());
            handler.ProvidePopup(popup);
            context.OnDestroy(() => popup?.Value?.Close());

            OnRecompose(context);
            
            content.Recompose(context);
            popupContent.Recompose(context);
            
            context.EndFrame();
        }

        public virtual bool StateLayoutEquals(IComponent other) => other is Popup;

        /// <summary>
        /// Method called during recompose. You can override it to store custom state.
        /// </summary>
        /// <param name="context"></param>
        protected virtual void OnRecompose(CompositionContext context) { }

        /// <summary>
        /// Inserts rendered popup content into the popup container.
        /// </summary>
        /// <param name="popupContentElement"></param>
        protected virtual void InsertMenuContent(VisualElement popupContentElement)
        {
            var popupContainer = PopupInstance.Container;

            if (popupContainer.childCount > 0)
            {
                if (popupContainer[0] == popupContentElement)
                    return;
                
                popupContainer.Clear();
            }

            popupContainer.Add(popupContentElement);
        }
    }
}