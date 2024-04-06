using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UI.Li.Utils;
using UnityEngine;
using UnityEngine.UIElements;
using CU = UI.Li.Utils.CompositionUtils;

using static UI.Li.Common.Layout.Layout;
using static UI.Li.Common.Common;

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
            
            return Row(labels.Select(Label)).WithStyle(tabListStyle);
            
            IComponent Label(IComponent text, int i) => label(text, () => onSelect(i), selected == i);
        }

        [NotNull]
        public static IComponent V(Label label, params (IComponent Label, Func<IComponent> Content)[] tabs) =>
            new Component(ctx =>
            {
                var cachedTabs = tabs.ToArray();
                
                var selected = ctx.Remember(0);

                return Col(
                    List(cachedTabs.Select(t => t.Label), i => selected.Value = i, selected.Value, label),
                    cachedTabs[selected.Value].Content().Id(selected.Value)
                );
            }, isStatic: true);

        [NotNull]
        public static IComponent V(params (IComponent Lable, Func<IComponent> Content)[] tabs) => V(null, tabs);

        private static IComponent DefaultLabel(IComponent content, Action onSelect, bool selected) => Button(onSelect, content).WithConditionalStyle(selected, selectedLabelStyle);

        private static readonly Style tabListStyle = new(alignContent: Align.FlexStart);

        private static readonly Style selectedLabelStyle = new(backgroundColor: new Color(0.5f, 0.5f, 0.5f));
    }
}