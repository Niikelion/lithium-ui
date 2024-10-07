using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UI.Li.Common;
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

namespace UI.Li.Editor.Debugging
{
    public class DebuggerWindow: ComposableWindow
    {
        private class ContextExtractor : Wrapper
        {
            [NotNull] private readonly Action<CompositionContext> onContextChanged;
            private CompositionContext currentContext;

            public ContextExtractor(IComponent innerComponent, [NotNull] Action<CompositionContext> onContextChanged) :
                base(innerComponent) => this.onContextChanged = onContextChanged;

            protected override void BeforeInnerRecompose(CompositionContext context)
            {
                base.BeforeInnerRecompose(context);
                if (context == currentContext)
                    return;

                currentContext = context;
                onContextChanged(context);
            }
        }
        
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
        
        [MenuItem("Lithium/Debugger")]
        public static void ShowDebuggerWindow() => GetWindow<DebuggerWindow>();
        protected override string WindowName => "Component Debugger";
        protected override bool HideContext => true;

        private CompositionContext self;

        protected override IComponent Layout() => new ContextExtractor(Root(), context => self = context);

        private IComponent Root() => WithState(() =>
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
                Switch(selectedContext.Value == null, null, DetailPanel),
                Hierarchy(selectedContext.Value),
                orientation: TwoPaneSplitViewOrientation.Horizontal,
                initialSize: 200,
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

        private static IComponent DetailPanel() => WithState(() =>
        {
            var node = Remember<CompositionContext.InspectedNode>(null);

            var selectionContext = UseContext<SelectionContext>();
            selectionContext.SetOnNodeChanged(newNode => node.Value = newNode);
            
            return Scroll(Let(node.Value, FullPanel, EmptyPanel));

            IComponent EmptyPanel() => Text("No component selected").WithStyle(new(padding: 4));

            IComponent FullPanel([NotNull] CompositionContext.InspectedNode n) => 
                Col(
                    Button(content: "Recompose", onClick: n.Recompose),
                    Col(n.Values.Select(Value))
                );
            IComponent Value(IMutableValue value, int i) => Row(Text($"{i}:"), StateVariable(value));
        });

        private static IComponent StateVariable(IMutableValue value) => WithState(() =>
        {
            var property = RememberF(() =>
            {
                var instance = CreateInstance<GenericProperty>();

                instance.property = value;
                
                var serializedObject = new SerializedObject(instance);

                return serializedObject.FindProperty("property");
            });
            
            ComponentState.OnDestroy(() =>
            {
                //TODO: check if on main thread
                //DestroyImmediate(property.Value.serializedObject.targetObject);
            });
            
            return Property(property.Value);
        });

        private static IComponent RenderNode(CompositionContext.InspectedNode node, int level = 0) => WithState(() =>
        {
            var selected = Remember(false);
            var selectionContext = UseContext<SelectionContext>();

            var defer = GetDeferrer();
            
            if (selected.Value)
                selectionContext.SetOnOldSelectionCleared(() => selected.Value = false);

            int offset = 13 * level;

            string name = $"{node.Name}{(node.Id > 0 ? $" #{node.Id}" : "")}";

            var children = node.Children;

            // NOTE: for some reason ui toolkit has a bug where height calculations are incorrect without wrapping element
            return Box(Switch(children.Count == 0, () =>
                Text(name, manipulators: new Clickable(OnSelected))
                    .WithStyle(new(flexGrow: 1, padding: new(left: offset)))
                    .WithStyle(textStyle)
                    .WithConditionalStyle(selected, selectedStyle), () =>
            {
                var content = children.Select((child, i) => RenderNode(child, level + 1).Id(i+1));

                return Foldout(
                    nobToggleOnly: true,
                    headerContainer: HeaderContainer,
                    contentContainer: ContentContainer,
                    header: Text(name, manipulators: new Clickable(OnSelected)).WithStyle(textStyle),
                    content: Col(content).WithStyle(fillStyle)
                ).WithStyle(fillStyle);
            }));
            
            void OnSelected()
            {
                if (selected.Value)
                    return;

                defer(() =>
                {
                    selected.Value = true;
                    selectionContext.SetNode(node); 
                });
            }

            IComponent HeaderContainer(IEnumerable<IComponent> content, Action onClick) => Row(
                    content: content,
                    manipulators: onClick?.Let(c => new Clickable(c))
                ).WithStyle(new(padding: new(left: offset), flexGrow: 1))
                .WithConditionalStyle(selected, selectedStyle);

            static IComponent ContentContainer(IComponent content, bool visible) =>
                content.WithStyle(new(display: visible ? DisplayStyle.Flex : DisplayStyle.None));
        });
        
        private List<CompositionContext> GetInstances() =>
            CompositionContext.Instances.Where(c => c != self).ToList();
        
        private static readonly Style fillStyle = new(flexGrow: 1);
        private static readonly Style toolbarDropdownStyle = new(width: 240);
        private static readonly Style topDividerStyle = new(height: 1, backgroundColor: new Color(0.16f, 0.16f, 0.16f));
        private static readonly Style componentPanelStyle = new(padding: 4);
        private static readonly Style centerItemsStyle = new(alignItems: Align.Center);
        private static readonly Style selectedStyle = new(backgroundColor: new Color(0.17f, 0.36f, 0.53f));
        private static readonly Style textStyle = new(color: Color.white);
    }

    [PublicAPI] public class GenericProperty : ScriptableObject
    {
        [SerializeReference] public object property;
    }
}