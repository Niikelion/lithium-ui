﻿using System;
using System.Collections.Generic;
using System.Linq;
using UI.Li.Common;
using UI.Li.Utils;
using UI.Li.Utils.Continuations;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

using static UI.Li.Common.Common;
using static UI.Li.Utils.Utils;
using static UI.Li.Common.Layout.Layout;
using static UI.Li.Fields.Fields;
using static UI.Li.ComponentState;

namespace UI.Li.Editor.Debugging
{
    public class OldDebuggerWindow: ComposableWindow
    {
        private struct SelectedNodeCtx
        {
            public int Id;
            public Action<CompositionContext.CompositionNode, int> OnSelect;
        }

        private class NodeIdCtx
        {
            public int Id;
        }
        
        [MenuItem("Lithium/Debugger(old)")]
        public static void ShowDebuggerWindow() => GetWindow<OldDebuggerWindow>();
        protected override string WindowName => "Old Component Debugger";

        protected override IComponent Layout() => WithState(() =>
        {
            var selectedContext = Remember<CompositionContext>(null);

            var instances = RememberF(GetInstances);
            var selectedNode = Remember<(CompositionContext.CompositionNode Node, int Id)>((null, -1));

            void OnSelect(CompositionContext.CompositionNode node, int id) => selectedNode.Value = (node, id);

            ProvideContext(new SelectedNodeCtx {
                Id = selectedNode.Value.Id,
                OnSelect = OnSelect
            });
            ProvideContext(new NodeIdCtx { Id = 1 });

            OnInit(() =>
            {
                CompositionContext.OnInstanceListChanged += AttachCallback;
                selectedContext.OnValueChanged += ResetSelection;

                return () =>
                {
                    selectedContext.OnValueChanged -= ResetSelection;
                    CompositionContext.OnInstanceListChanged -= AttachCallback;
                };

                void AttachCallback() => instances.Value = GetInstances();
                void ResetSelection() => selectedNode.Value = (null, -1);
            });
            
            var hierarchy = selectedContext.Value?.InspectHierarchy()?.ToArray() ?? Array.Empty<CompositionContext.CompositionNode>();

            var toolbar = Row(
                Dropdown(
                    initialValue: instances.Value.IndexOf(selectedContext.Value) + 1,
                    options: instances.Value.Select(instance => instance.Name).Prepend("None").ToList(),
                    onSelectionChanged: i => selectedContext.Value = i == 0 ? null : instances.Value[i - 1]
                ).WithStyle(toolbarDropdownStyle)
            ).WithStyle(centerItemsStyle);
            
            var content = SplitArea(
                Switch(selectedContext.Value == null, RenderNoDetails, DetailPanel),
                Switch(selectedContext.Value == null, RenderNoPanel, DisplayHierarchy),
                orientation: TwoPaneSplitViewOrientation.Horizontal,
                initialSize: 200,
                reverse: true
            ).WithStyle(fillStyle);
            
            return Col(toolbar, content).WithStyle(fillStyle);

            List<CompositionContext> GetInstances() =>
                CompositionContext.Instances.Where(c => c.Name != WindowName).ToList();

            IComponent DisplayHierarchy() =>
                Hierarchy(hierarchy);

            IComponent Value(IMutableValue value, int i) =>
                Text($"{i}: {value}");
            
            IComponent DetailPanel() =>
                Col(selectedNode.Value.Node?.Values?.Select(Value) ?? Enumerable.Empty<IComponent>());
        }, isStatic: true);
        
        private static IComponent RenderNoPanel() => Text("No panel selected.");

        private static IComponent RenderNoDetails() => null;

        private static IComponent Hierarchy(CompositionContext.CompositionNode[] roots) => WithState(() =>
            Scroll(Col(roots.Select(root => RenderNode(root)))
        ), isStatic: true);
        
        private static IComponent RenderNode(CompositionContext.CompositionNode node, int level = 0)
        {
            var idCtx = UseContext<NodeIdCtx>();
            var selCtx = UseContext<SelectedNodeCtx>();
            bool selected = selCtx.Id == idCtx.Id;
            
            int currentId = idCtx.Id++;
            
            int offset = 13 * level;

            return Box(RenderNodeContent()).Id(currentId);
            
            IComponent RenderNodeContent()
            {
                string name = $"{node.Name}{(node.Id > 0 ? $" #{node.Id}" : "")}";

                var children = node.Children;
                if (children.Count == 0)
                {
                    return Text(name, manipulators: new Clickable(OnSelected))
                        .WithStyle(new (flexGrow: 1, padding: new (left: offset)))
                        .WithStyle(textStyle)
                        .WithConditionalStyle(selected, selectedStyle).Id(1);
                }

                var content = children.Select(child => RenderNode(child, level + 1));

                return Foldout(
                    nobToggleOnly: true,
                    headerContainer: HeaderContainer,
                    contentContainer: ContentContainer,
                    header: Text(name, manipulators: new Clickable(OnSelected))
                        .WithStyle(textStyle),
                    content: Col(content).WithStyle(fillStyle)
                ).WithStyle(fillStyle).Id(2);
            }

            void OnSelected() => selCtx.OnSelect?.Invoke(node, currentId);
            
            IComponent HeaderContainer(IEnumerable<IComponent> content, Action onClick) => Row(
                content: content,
                manipulators: onClick?.Let(c => new Clickable(c))
            ).WithStyle(new (padding: new (left: offset), flexGrow: 1))
                .WithConditionalStyle(selected, selectedStyle);
            
            static IComponent ContentContainer(IComponent content, bool visible) =>
                content.WithStyle(new (display: visible ? DisplayStyle.Flex : DisplayStyle.None));
        }

        private static readonly Style fillStyle = new(flexGrow: 1);
        private static readonly Style toolbarDropdownStyle = new(minWidth: 240);
        private static readonly Style centerItemsStyle = new(alignItems: Align.Center);
        private static readonly Style selectedStyle = new(backgroundColor: new Color(0.17f, 0.36f, 0.53f));
        private static readonly Style textStyle = new(color: Color.white);
    }
}