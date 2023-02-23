using Codice.Client.Common.TreeGrouper;
using UI.Li.Common.Units;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using CU = UI.Li.Utils.CompositionUtils;

namespace UI.Li.Editor.Debugging
{
    public class DebuggerWindow: ComposableWindow
    {
        [MenuItem("Lithium/Debugger")]
        public static void ShowJsonWindow()
        {
            EditorWindow wnd = GetWindow<DebuggerWindow>();
            wnd.titleContent = new GUIContent("Composition Debugger");
        }

        protected override string WindowName => "Composition Debugger";

        protected override IComposition Layout() => new Composition(ctx =>
        {
            List<CompositionContext> GetInstances() =>
                CompositionContext.Instances.Where(c => c.Name != WindowName).ToList();
        
            var selectedContext = ctx.Remember<CompositionContext>(null);

            var instances = ctx.RememberF(GetInstances);
            var selectedNode = ctx.Remember(-1);

            ctx.OnInit(() =>
            {
                void AttachCallback() => instances.Value = GetInstances();

                CompositionContext.OnInstanceListChanged += AttachCallback;

                return () => CompositionContext.OnInstanceListChanged -= AttachCallback;
            });
            
            var hierarchy = selectedContext.Value?.InspectHierarchy()?.ToArray() ?? Array.Empty<CompositionContext.CompositionNode>();

            IComposition Toolbar() =>
                CU.Flex(
                    direction: FlexDirection.Row,
                    data: new(
                        alignItems: Align.Center
                    ),
                    content: new IComposition[]
                    {
                        CU.Dropdown(
                            data: new(
                                minWidth: 240
                            ),
                            initialValue: instances.Value.IndexOf(selectedContext.Value)+1,
                            options: instances.Value.Select(instance => instance.Name).Prepend("None").ToList(),
                            onSelectionChanged: i => selectedContext.Value = i == 0 ? null : instances.Value[i - 1]
                        )
                    }
                );

            var content = new IComposition[hierarchy.Length];

            int nodeId = 1;

            void OnNodeSelected(int id)
            {
                selectedNode.Value = id;
            }
            
            for (int i = 0; i < content.Length; ++i)
                content[i] = CU.WithId(nodeId,  CU.Box(RenderNode(hierarchy[i], ref nodeId, OnNodeSelected)));
            
            IComposition DisplayHierarchy() =>
                CU.Flex(
                    direction: FlexDirection.Column,
                    content: content
                );

            IComposition Content() =>
                CU.Switch(selectedContext.Value == null, RenderNoPanel, DisplayHierarchy);
                

            return CU.Flex(
                direction: FlexDirection.Column,
                content: new[] { CU.WithId(1, Toolbar()), CU.WithId(2, Content()) }
            );
        }, isStatic: true);
        
        private static IComposition RenderNoPanel() => CU.Text("No panel selected.", new(flexGrow: 1));

        private static IComposition RenderNode(CompositionContext.CompositionNode node, ref int id, Action<int> onSelect)
        {
            int currentId = id++;

            string name = $"{node.Name}{(node.Id > 0 ? $" #{node.Id}" : "")}";
            
            var children = node.Children;
            if (children.Count == 0)
            {
                return CU.WithId(1, CU.Text(
                    name,
                    data: new(flexGrow: 1)
                ));
            }

            var content = new IComposition[ children.Count ];

            int i = 0;
            foreach (var child in children)
            {
                content[i] = RenderNode(child, ref id, onSelect);
                ++i;
            }
                
            return CU.WithId(2, CU.Foldout(
                data: new(flexGrow: 1),
                header: CU.Text(name, data: new( onClick: () => onSelect(currentId) )),
                content: CU.Flex(
                    direction: FlexDirection.Column,
                    data: new(padding: new(left: 2.Px()), flexGrow: 1),
                    content: content
                )
            ));
        }
    }
}