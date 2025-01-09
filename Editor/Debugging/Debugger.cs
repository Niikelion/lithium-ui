using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UI.Li.Common;
using UI.Li.Editor.Internal;
using UI.Li.Utils;
using UI.Li.Utils.Continuations;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

using static UI.Li.Utils.Utils;
using static UI.Li.Common.Common;
using static UI.Li.Fields.Fields;
using static UI.Li.Common.Layout.Layout;
using static UI.Li.ComponentState;
using static UI.Li.Editor.Fields;
using static UI.Li.Utils.EventUtils;

namespace UI.Li.Editor.Debugging
{
    public class DebuggerWindow: ComposableWindow
    {
        private class SelectionContext
        {
            private CompositionContext.InspectedNode currentNode;
            private Action<CompositionContext.InspectedNode> setNode;
            private Action clearOldSelection;

            public void SetNode(CompositionContext.InspectedNode node)
            {
                if (node == currentNode)
                    return;

                currentNode = node;
                setNode?.Invoke(node);
                clearOldSelection?.Invoke();
            }

            public void SetOnNodeChanged(Action<CompositionContext.InspectedNode> onNodeChanged) =>
                setNode = onNodeChanged;

            public void SetOnOldSelectionCleared(Action onOldSelectionCleared) =>
                clearOldSelection = onOldSelectionCleared;
        }

        private const int InitialStatePanelSize = 500;

        [MenuItem("Lithium/Debugger")]
        public static void ShowDebuggerWindow() => GetWindow<DebuggerWindow>();
        protected override string WindowName => "Component Debugger";
        protected override bool HideContext => true;

        protected override IComponent Layout() => Root();

        private static IComponent Root() => WithState(() =>
        {
            var selectedContext = Remember<CompositionContext>(null);
            var updater = RememberRef(true);

            var instances = RememberF(GetInstances);

            var selectionContext = new SelectionContext();
            
            ProvideContext(selectionContext);

            OnInit(() =>
            {
                var lastContext = selectedContext.Value;

                if (lastContext != null)
                    lastContext.OnUpdate += HandleContextUpdate;
                
                CompositionContext.OnInstanceListChanged += RefreshList;
                selectedContext.OnValueChanged += HandleNewContext;
                
                return () =>
                {
                    selectedContext.OnValueChanged -= HandleNewContext;
                    CompositionContext.OnInstanceListChanged -= RefreshList;
                };
                
                void RefreshList() => instances.Value = GetInstances();
                void HandleNewContext()
                {
                    selectionContext.SetNode(null);

                    if (lastContext != null)
                        lastContext.OnUpdate -= HandleContextUpdate;
                    
                    lastContext = selectedContext.Value;

                    if (lastContext != null)
                        lastContext.OnUpdate += HandleContextUpdate;
                }

                void HandleContextUpdate() => updater.NotifyChanged();
            });

            var toolbar = Row(
                Dropdown(
                    initialValue: instances.Value.IndexOf(selectedContext.Value) + 1,
                    options: instances.Value.Select((instance, i) => $"{i}: {instance.Name}").Prepend("None").ToList(),
                    onSelectionChanged: i => selectedContext.Value = i == 0 ? null : instances.Value[i - 1]
                ).WithStyle(toolbarDropdownStyle)
            ).WithStyle(centerItemsStyle);
            
            var content = SplitArea(
                Hierarchy(selectedContext.Value),
                Switch(selectedContext.Value == null, null, StatePanel),
                orientation: TwoPaneSplitViewOrientation.Horizontal,
                initialSize: InitialStatePanelSize,
                reverse: true
            ).WithStyle(fillStyle);
            
            return Col(toolbar, Box().WithStyle(topDividerStyle), content).WithStyle(fillStyle).Id(selectedContext.Value?.GetHashCode() ?? 1);
        }, isStatic: true);

        private static IComponent Hierarchy([CanBeNull] CompositionContext context)
        {
            var roots = context?.InspectHierarchy()?.ToArray();

            return Scroll(Let(roots, WithContent, WithoutContent).WithStyle(componentPanelStyle));

            IComponent WithContent(CompositionContext.InspectedNode[] r) =>
                Col(r.Select((root, i) => RenderNode(root).Id(i+1)));
            
            IComponent WithoutContent() => Text("No panel selected");
        }
        
        private static IComponent StatePanel() => WithState(() =>
        {
            var node = Remember<CompositionContext.InspectedNode>(null);

            var selectionContext = UseContext<SelectionContext>();
            selectionContext.SetOnNodeChanged(newNode => node.Value = newNode);
            
            return Let(node.Value, FullPanel, EmptyPanel);

            IComponent EmptyPanel() => Text("No component selected").WithStyle(new(padding: 4));

            IComponent FullPanel([NotNull] CompositionContext.InspectedNode n)
            {
                var values = n.Values;
                var contexts = n.Contexts;
                
                return Col(
                    Button(content: "Recompose", onClick: n.Recompose),
                    Scroll(Col(
                            If(values.Count > 0, () => Col(
                                Text("State:"),
                                Col(values.Select(Value)).WithStyle(leftPad)
                            )),
                            If(contexts.Count > 0, () => Col(
                                Text("Contexts:"),
                                Col(contexts.Select(Context)).WithStyle(leftPad)
                            ))
                        )
                    )
                );
            }

            IComponent Value(IMutableValue value, int i)
            {
                return Row(Text($"{i}:"), ValueInspector(value)).Id(i + 1);
            }

            IComponent Context(KeyValuePair<Type, object> context, int i) =>
                Row(Text($"{context.Key.FullName}:"), ValueInspector(context.Value)).Id(i+1);
        });

        private static IComponent ValueInspector(object value) => WithState(() =>
        {
            var property = Cache<SerializedProperty>(oldProperty =>
            {
                if (oldProperty != null)
                {
                    GenericProperty.Return(oldProperty.serializedObject.targetObject as GenericProperty);
                }

                var instance = GenericProperty.Get();
                instance.property = value;
                
                return instance.SerializedObject.FindProperty("property");
            }, value);
            
            return Property(property);
        });

        private static IComponent RenderNode(CompositionContext.InspectedNode node, int level = 0) => WithState(() =>
        {
            var selected = Remember(false);
            var selectionContext = UseContext<SelectionContext>();
            
            if (selected.Value)
                selectionContext.SetOnOldSelectionCleared(() => selected.Value = false);

            int offset = 13 * level;

            string name = $"{node.Name}{(node.Id > 0 ? $" #{node.Id}" : "")}";

            var children = node.Children;

            //NOTE: for some reason ui toolkit has a bug where height calculations are incorrect without wrapping element
            return Box(Switch(children.Count == 0, () =>
                Text(name, manipulators: OnClick(OnSelected))
                    .WithStyle(new(flexGrow: 1, padding: new(left: offset)))
                    .WithStyle(textStyle)
                    .WithConditionalStyle(node.Crashed, crashedTextStyle)
                    .WithConditionalStyle(selected, selectedStyle), () =>
            {
                var content = children.Select((child, i) => RenderNode(child, level + 1).Id(i+1));

                return Foldout(
                    nobToggleOnly: true,
                    headerContainer: HeaderContainer,
                    contentContainer: ContentContainer,
                    header: Text(name, manipulators: OnClick(OnSelected)).WithStyle(textStyle).WithConditionalStyle(node.Crashed, crashedTextStyle),
                    content: Col(content).WithStyle(fillStyle)
                ).WithStyle(fillStyle);
            }));
            
            void OnSelected()
            {
                if (selected.Value)
                    return;

                selected.Value = true;
                selectionContext.SetNode(node); 
            }

            IComponent HeaderContainer(IEnumerable<IComponent> content, Action onClick) => Row(
                    content: content,
                    manipulators: onClick?.Let(OnClick)
                ).WithStyle(new(padding: new(left: offset), flexGrow: 1))
                .WithConditionalStyle(selected, selectedStyle);

            static IComponent ContentContainer(IComponent content, bool visible) =>
                content.WithStyle(new(display: visible ? DisplayStyle.Flex : DisplayStyle.None));
        });
        
        private static List<CompositionContext> GetInstances() => 
            CompositionContext.Instances.ToList();
        
        private static readonly Style fillStyle = new(flexGrow: 1);
        private static readonly Style toolbarDropdownStyle = new(width: 240);
        private static readonly Style topDividerStyle = new(height: 1, backgroundColor: new Color(0.16f, 0.16f, 0.16f));
        private static readonly Style componentPanelStyle = new(padding: 4);
        private static readonly Style centerItemsStyle = new(alignItems: Align.Center);
        private static readonly Style selectedStyle = new(backgroundColor: new Color(0.17f, 0.36f, 0.53f));
        private static readonly Style textStyle = new(color: Color.white);
        private static readonly Style crashedTextStyle = new(color: Color.red);
        private static readonly Style leftPad = new(padding: new(left: 8));
    }
}