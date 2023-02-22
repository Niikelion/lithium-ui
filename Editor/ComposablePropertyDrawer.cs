using UI.Li.Utils;
using UnityEditor;
using JetBrains.Annotations;
using UnityEngine.UIElements;
using System.Collections.Generic;

using CU = UI.Li.Utils.CompositionUtils;

namespace UI.Li.Editor
{
    [PublicAPI] public abstract class ComposablePropertyDrawer: PropertyDrawer
    {
        private List<CompositionRenderer> renderers = new ();
        
        public sealed override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var renderer = new CompositionRenderer(Layout(property), DrawerName);

            renderers.Add(renderer);
            
            var container = new VisualElement();
            
            container.Add(renderer.UpdateAndRender());

            return container;
        }

        protected abstract IComposition Layout(SerializedProperty property);

        protected virtual string DrawerName => GetType().Name;
        
        ~ComposablePropertyDrawer()
        {
            foreach (var renderer in renderers)
                renderer.Dispose();
            
            renderers.Clear();
        }
    }
}