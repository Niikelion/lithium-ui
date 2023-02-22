using UI.Li.Utils;
using UnityEditor;
using JetBrains.Annotations;

namespace UI.Li.Editor
{
    /// <summary>
    /// Base class for <see cref="EditorWindow"/>s using composables system.
    /// </summary>
    public abstract class ComposableWindow: EditorWindow
    {
        private CompositionRenderer renderer;
        
        public void CreateGUI()
        {
            var content = GetRenderer().UpdateAndRender();
            
            rootVisualElement.Add(content);
        }

        /// <summary>
        /// Method used to render window.
        /// </summary>
        /// <returns>Composition to be used as windows content</returns>
        [PublicAPI] [NotNull] protected abstract IComposition Layout();

        protected virtual void OnDestroy() => renderer?.Dispose();

        protected virtual string WindowName => GetType().Name;
        
        private CompositionRenderer GetRenderer() => renderer ??= new CompositionRenderer(Layout(), WindowName);
    }
}