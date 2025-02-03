using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace UI.Li.Common.Layout
{
    /// <summary>
    /// Dropdown popup
    /// </summary>
    [PublicAPI]
    public sealed class DropdownPopup : Popup
    {
        [PublicAPI]
        public class Handler : PopupHandler
        {
            private VisualElement anchor;
            
            /// <summary>
            /// Opens the dropdown in the specified direction relative to the content component.
            /// </summary>
            /// <param name="direction"></param>
            public void Open(UIElements.Popup.Direction direction)
            {
                if (anchor == null || PopupInstance == null)
                    return;
                
                PopupInstance.OpenDropdown(anchor, direction);
            }
            
            internal void ProvideAnchor(VisualElement anchorElement) => anchor = anchorElement;
        }
        
        private Handler handler;

        [NotNull] public static Handler Provide() => new();

        [NotNull]
        public static DropdownPopup V(
            [NotNull] IComponent content,
            [NotNull] Handler handler,
            [NotNull] IComponent popupContent
        ) => new(content, handler, popupContent);

        private DropdownPopup(
            [NotNull] IComponent content,
            [NotNull] Handler handler,
            [NotNull] IComponent popupContent
        ) : base(content, handler, popupContent) => this.handler = handler;

        public override VisualElement Render()
        {
            var root = base.Render();
            handler.ProvideAnchor(root);
            return root;
        }
    }
}