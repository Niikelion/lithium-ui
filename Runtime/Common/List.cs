using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UI.Li.Utils;
using UnityEngine;
using UnityEngine.UIElements;

using CU = UI.Li.Utils.CompositionUtils;

namespace UI.Li.Common
{
    //TODO: implement list
    /// <summary>
    /// Component representing list of sub-components.
    /// </summary>
    /// <remarks>WIP, use at your own risk!</remarks>
    [PublicAPI] [Obsolete("WIP")] public class List
    {
        private struct ValueComparer : IComparer<KeyValuePair<IComponent, int>>
        {
            public int Compare(KeyValuePair<IComponent, int> x, KeyValuePair<IComponent, int> y)
            {
                return x.Value.CompareTo(y.Value);
            }
        }

        [NotNull]
        private static IComponent V(int initiallyVisibleElements = 30, params IComponent[] content) => new Component(ctx =>
        {
            var fullRect = ctx.RememberRef(Rect.zero);
            var childRects = ctx.RememberRef(new List<Rect>());
            var firstElement = ctx.Remember(0);
            var lastElement = ctx.Remember(Math.Min(initiallyVisibleElements, content.Length));
            
            var elements = content
                .Select((v, i) => (v, i))
                .Where((_, i) => i >= firstElement.Value && i <= lastElement.Value)
                .Select(v => CU.Box(v.v, manipulators: new SizeWatcher(rect =>
                {
                    //TODO: save rect
                })));
            
            return CU.Scroll(CU.Flex(content: elements), manipulators: new SizeWatcher(rect =>
            {
                fullRect.Value = rect;
            }));
        });
    }
}