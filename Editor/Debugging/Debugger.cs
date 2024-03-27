using System;
using System.Collections.Generic;
using System.Linq;
using UI.Li.Common;
using UI.Li.Utils;
using UI.Li.Utils.Continuations;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using CU = UI.Li.Utils.CompositionUtils;

namespace UI.Li.Editor.Debugging
{
    public class DebuggerWindow: ComposableWindow
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
        
        [MenuItem("Lithium/Debugger")]
        public static void ShowDebuggerWindow() => GetWindow<DebuggerWindow>();
        protected override string WindowName => "Component Debugger";

        protected override IComponent Layout() => new Component(ctx =>
        {
            var selectedContext = ctx.Remember<CompositionContext>(null);

            var instances = ctx.RememberF(GetInstances);
            var selectedNode = ctx.Remember<(CompositionContext.CompositionNode Node, int Id)>((null, -1));

            void OnSelect(CompositionContext.CompositionNode node, int id) => selectedNode.Value = (node, id);

            ctx.ProvideContext(new SelectedNodeCtx {
                Id = selectedNode.Value.Id,
                OnSelect = OnSelect
            });
            ctx.ProvideContext(new NodeIdCtx { Id = 1 });

            ctx.OnInit(() =>
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
            
            return CU.Flex(
                direction: FlexDirection.Column,
                content: CU.Seq(Toolbar(), Content())
            ).WithStyle(fillStyle);

            List<CompositionContext> GetInstances() =>
                CompositionContext.Instances.Where(c => c.Name != WindowName).ToList();

            IComponent Toolbar() =>
                CU.Flex(
                    direction: FlexDirection.Row,
                    content: IComponent.Seq(CU.Dropdown(
                        initialValue: instances.Value.IndexOf(selectedContext.Value)+1,
                        options: instances.Value.Select(instance => instance.Name).Prepend("None").ToList(),
                        onSelectionChanged: i => selectedContext.Value = i == 0 ? null : instances.Value[i - 1]
                    ).WithStyle(toolbarDropdownStyle))
                ).WithStyle(centerItemsStyle);

            IComponent DisplayHierarchy() => Hierarchy(hierarchy);

            IComponent Content() =>
                CU.SplitArea(
                    CU.Switch(selectedContext.Value == null, RenderNoDetails, DetailPanel),
                    CU.Switch(selectedContext.Value == null, RenderNoPanel, DisplayHierarchy),
                    orientation: TwoPaneSplitViewOrientation.Horizontal,
                    initialSize: 200,
                    reverse: true
                ).WithStyle(fillStyle);

            IComponent Value(IMutableValue value, int i) => CU.Text($"{i}: {value}");
            
            IComponent DetailPanel()
            {
                var values = selectedNode.Value.Node?.Values?.Select(Value);
                
                return CU.Switch(values != null,
                    () => CU.Column(CU.Seq(values)),
                    () => CU.Box()
                );
            }
        }, isStatic: true);
        
        private static IComponent RenderNoPanel() => CU.Text("No panel selected.");

        private static IComponent RenderNoDetails() => CU.Box();
        
        private static IComponent Hierarchy(CompositionContext.CompositionNode[] roots) => new Component(ctx =>
        {
            return CU.Scroll(
                CU.Column(
                    roots.Select(root => RenderNode(root, ctx))
                )
            );
        }, isStatic: true);
        
        private static IComponent RenderNode(CompositionContext.CompositionNode node, ComponentState ctx, int level = 0)
        {
            var idCtx = ctx.UseContext<NodeIdCtx>();
            var selCtx = ctx.UseContext<SelectedNodeCtx>();
            bool selected = selCtx.Id == idCtx.Id;
            
            int currentId = idCtx.Id++;
            
            int offset = 13 * level;

            return CU.Box(RenderNodeContent()).Id(currentId);
            
            IComponent RenderNodeContent()
            {
                string name = $"{node.Name}{(node.Id > 0 ? $" #{node.Id}" : "")}";

                var children = node.Children;
                if (children.Count == 0)
                {
                    return CU.Text(name, manipulators: new Clickable(OnSelected))
                        .WithStyle(new (flexGrow: 1, padding: new (left: offset)))
                        .WithStyle(textStyle)
                        .WithConditionalStyle(selected, selectedStyle).Id(1);
                }

                var content = children.Select(child => RenderNode(child, ctx, level + 1));

                return CU.Foldout(
                    nobToggleOnly: true,
                    headerContainer: HeaderContainer,
                    contentContainer: ContentContainer,
                    header: CU.Text(name, manipulators: new Clickable(OnSelected))
                        .WithStyle(textStyle),
                    content: CU.Column(content).WithStyle(fillStyle)
                ).WithStyle(fillStyle).Id(2);
            }

            void OnSelected() => selCtx.OnSelect?.Invoke(node, currentId);
            
            IComponent HeaderContainer(IEnumerable<IComponent> content, Action onClick) => CU.Flex(
                direction: FlexDirection.Row,
                content: content,
                manipulators: onClick?.Let(c => new Clickable(c))
            ).WithStyle(new (padding: new (left: offset), flexGrow: 1))
                .WithConditionalStyle(selected, selectedStyle);
            
            static IComponent ContentContainer(IComponent content, bool visible) => CU.Box(content)
                .WithStyle(new (display: visible ? DisplayStyle.Flex : DisplayStyle.None));
        }

        private static readonly Style fillStyle = new(flexGrow: 1);
        private static readonly Style toolbarDropdownStyle = new(minWidth: 240);
        private static readonly Style centerItemsStyle = new(alignItems: Align.Center);
        private static readonly Style selectedStyle = new(backgroundColor: new Color(0.17f, 0.36f, 0.53f));
        private static readonly Style textStyle = new(color: Color.white);
    }
}