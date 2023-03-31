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
        private ComponentRenderer renderer;
        
        public void CreateGUI()
        {
            var content = GetRenderer().UpdateAndRender();
            
            rootVisualElement.Add(content);
        }

        /// <summary>
        /// Method used to render window.
        /// </summary>
        /// <returns>Component to be used as windows content</returns>
        [PublicAPI] [NotNull] protected abstract IComponent Layout();

        protected virtual void OnDestroy() => renderer?.Dispose();

        protected virtual string WindowName => GetType().Name;
        
        private ComponentRenderer GetRenderer() => renderer ??= new ComponentRenderer(Layout(), WindowName);
    }
}