using UnityEngine;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace UI.Li.Utils
{
    [RequireComponent(typeof(UIDocument))]
    public abstract class ComposableDocument: MonoBehaviour
    {
        private UIDocument document;
        private new ComponentRenderer renderer;

        [PublicAPI]
        protected abstract IComponent Layout();
        
        protected virtual void Awake() => PopulateDocument();

        protected virtual void OnDestroy() => renderer?.Dispose();

        private void PopulateDocument()
        {
            if (document != null)
                return;

            document = GetComponent<UIDocument>();

            renderer?.Dispose();

            renderer = new(Layout);
            
            document.rootVisualElement.Clear();
            document.rootVisualElement.Add(renderer.UpdateAndRender());
        }
    }
}