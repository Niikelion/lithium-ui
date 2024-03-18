using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;
using CU = UI.Li.Utils.CompositionUtils;

namespace UI.Li.Common
{
    [PublicAPI]
    public static class Tabs
    {
        public delegate IComponent Label(IComponent content, Action onSelect, bool selected);
        
        [NotNull]
        public static IComponent List(IEnumerable<IComponent> labels, Action<int> onSelect, int selected, Label label = null) 
        {
            label ??= DefaultLabel;
            
            return CU.Flex(direction: FlexDirection.Row, content: labels.Select(Label)).S(TabListStyle);
            
            IComponent Label(IComponent text, int i) => label(text, () => onSelect(i), selected == i);
        }

        [NotNull]
        public static IComponent V(Label label, params (IComponent Label, Func<IComponent> Content)[] tabs) =>
            new Component(ctx =>
            {
                var cachedTabs = tabs.ToArray();
                
                var selected = ctx.Remember(0);
                
                return CU.Flex(direction: FlexDirection.Column, content: IComponent.Seq(
                    List(cachedTabs.Select(t => t.Label), i => selected.Value = i, selected.Value, label),
                    CU.WithId(selected.Value, cachedTabs[selected.Value].Content())
                ));
            }, isStatic: true);

        [NotNull]
        public static IComponent V(params (IComponent Lable, Func<IComponent> Content)[] tabs) => V(null, tabs);

        private static IComponent DefaultLabel(IComponent content, Action onSelect, bool selected) => CU.Button(onSelect, content).Cs(selected, SelectedLabelStyle);
        
        private static readonly StyleFunc TabListStyle = CU.Styled(new Style
        {
            AlignContent = Align.FlexStart
        });

        private static readonly StyleFunc SelectedLabelStyle = CU.Styled(new Style
        {
            BackgroundColor = new Color(0.5f, 0.5f, 0.5f)
        });
    }
}