using UI.Li.Utils;
using UnityEditor;
using JetBrains.Annotations;
using UnityEngine.UIElements;
using System.Collections.Generic;

namespace UI.Li.Editor
{
    [PublicAPI] public abstract class ComposablePropertyDrawer: PropertyDrawer
    {
        private List<ComponentRenderer> renderers = new();
        
        public sealed override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var renderer = new ComponentRenderer(() => Layout(property), DrawerName, HideContext);

            renderers.Add(renderer);
            
            var container = new VisualElement();
            
            container.Add(renderer.UpdateAndRender());

            if (HideContext) return container;
            
            container.RegisterCallback<DetachFromPanelEvent>(_ => renderer.MakeHidden());
            container.RegisterCallback<AttachToPanelEvent>(_ => renderer.MakeHidden(false));

            return container;
        }

        protected abstract IComponent Layout(SerializedProperty property);

        protected virtual string DrawerName => GetType().Name;

        protected virtual bool HideContext => false;
        
        ~ComposablePropertyDrawer()
        {
            foreach (var renderer in renderers)
                renderer.Dispose();
            
            renderers.Clear();
        }
    }
}