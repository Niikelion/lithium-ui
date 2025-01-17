﻿using UI.Li.Utils;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace UI.Li.Editor
{
    [PublicAPI] public abstract class ComposableEditor: UnityEditor.Editor
    {
        private ComponentRenderer renderer;
        
        public sealed override VisualElement CreateInspectorGUI()
        {
            renderer?.Dispose();
            renderer = new (Layout, EditorName, HideContext);

            return renderer.UpdateAndRender();
        }

        protected virtual void OnDestroy() => renderer?.Dispose();

        protected virtual string EditorName => GetType().Name;
        protected virtual bool HideContext => false;
        
        protected abstract IComponent Layout();
    }
}