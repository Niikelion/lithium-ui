using System;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace UI.Li.Common.UIElements
{
    [PublicAPI]
    public class Popup
    {
        public bool IsOpen => root != null;
        public VisualElement Container { get; private set; } = new();
        private Action cleanup;

        public enum Direction
        {
            Down,
            Up,
            Left,
            Right
        }
        
        private VisualElement root;
        private VisualElement wrapper = new() { style = { position = Position.Absolute, display = DisplayStyle.None } };

        public Popup()
        {
            wrapper.Add(Container);
            wrapper.AddToClassList(GenericDropdownMenu.containerOuterUssClassName);
        }

        public void OpenDropdown(VisualElement anchor, Direction direction, VisualElement rootElement = null)
        {
            if (IsOpen) Close();

            root = rootElement ?? FindRoot(anchor.panel);
            root.Add(wrapper);

            wrapper.style.display = DisplayStyle.Flex;
            
            var rect = root.WorldToLocal(anchor.worldBound);
            
            switch (direction)
            {
                default:
                case Direction.Down:
                {
                    wrapper.style.top = rect.yMax;
                    wrapper.style.bottom = StyleKeyword.Null;
                    
                    wrapper.RegisterCallback<GeometryChangedEvent>(CenterHorizontally);
                    cleanup = () => wrapper.UnregisterCallback<GeometryChangedEvent>(CenterHorizontally);

                    return;
                }
                case Direction.Up:
                {
                    wrapper.style.top = StyleKeyword.Null;
                    wrapper.style.bottom = rect.yMin;
                    
                    wrapper.RegisterCallback<GeometryChangedEvent>(CenterHorizontally);
                    cleanup = () => wrapper.UnregisterCallback<GeometryChangedEvent>(CenterHorizontally);
                    
                    return;
                }
                case Direction.Right:
                {
                    wrapper.style.left = rect.xMax;
                    wrapper.style.right = StyleKeyword.Null;
                    
                    wrapper.RegisterCallback<GeometryChangedEvent>(CenterVertically);
                    cleanup = () => wrapper.UnregisterCallback<GeometryChangedEvent>(CenterVertically);
                    
                    return;
                }
                case Direction.Left:
                {
                    wrapper.style.left = StyleKeyword.Null;
                    wrapper.style.right = rect.xMin;
                    
                    wrapper.RegisterCallback<GeometryChangedEvent>(CenterVertically);
                    cleanup = () => wrapper.UnregisterCallback<GeometryChangedEvent>(CenterVertically);
                    
                    return;
                }
            }
            
            void CenterHorizontally(GeometryChangedEvent _)
            {
                var wrapperRect = root.WorldToLocal(wrapper.worldBound);
                var anchorRect = root.WorldToLocal(anchor.worldBound);
                
                wrapper.style.left = Math.Max(anchorRect.xMin + (anchorRect.width - wrapperRect.width) / 2, 0);
                wrapper.style.right = StyleKeyword.Null;
            }

            void CenterVertically(GeometryChangedEvent _)
            {
                var wrapperRect = root.WorldToLocal(wrapper.worldBound);
                var anchorRect = root.WorldToLocal(anchor.worldBound);
                
                wrapper.style.top = Math.Max(anchorRect.yMin + (anchorRect.height - wrapperRect.height) / 2, 0);
                wrapper.style.bottom = StyleKeyword.Null;
            }
        }

        public void OpenModal(IPanel panel, VisualElement rootElement = null)
        {
            if (IsOpen) Close();
            
            root = rootElement ?? FindRoot(panel);
            
            root.Add(wrapper);
            
            wrapper.style.display = DisplayStyle.Flex;

            wrapper.RegisterCallback<GeometryChangedEvent>(Center);
            cleanup = () => wrapper.UnregisterCallback<GeometryChangedEvent>(Center);

            return;
            
            void Center(GeometryChangedEvent _)
            {
                var wrapperRect = root.WorldToLocal(wrapper.worldBound);
                var panelRect = root.layout;
                
                wrapper.style.top = Math.Max((panelRect.height - wrapperRect.height) / 2, 0);
                wrapper.style.left = Math.Max((panelRect.width - wrapperRect.width) / 2, 0);
            }
        }
        
        public void Close()
        {
            cleanup?.Invoke();
            if (wrapper.parent == root) root?.Remove(wrapper);
            wrapper.style.display = DisplayStyle.None;
            cleanup = null;
        }

        private static VisualElement FindRoot(IPanel panel) => panel.visualTree[panel.visualTree.childCount-1];
    }
}