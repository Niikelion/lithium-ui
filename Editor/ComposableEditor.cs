using UI.Li.Utils;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace UI.Li.Editor
{
    [PublicAPI] public abstract class ComposableEditor: UnityEditor.Editor
    {
        private CompositionRenderer renderer;
        
        public sealed override VisualElement CreateInspectorGUI()
        {
            renderer?.Dispose();
            renderer = new CompositionRenderer(Layout(), EditorName);

            return renderer.UpdateAndRender();
        }

        protected virtual void OnDestroy() => renderer?.Dispose();

        protected virtual string EditorName => GetType().Name;
        
        protected abstract IComposition Layout();
    }
}