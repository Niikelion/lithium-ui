using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UI.Li.Utils;
using UnityEngine.UIElements;

using static UI.Li.Common.Common;

namespace UI.Li.Common.Layout
{
    //TODO: allow for dynamic content creation (aka. creating elements on the fly instead of up front)
    //TODO: maybe we can do some magic to allow dynamic elements addition without too much overhead?
    /// <summary>
    /// Component representing optimized list of components.
    /// </summary>
    [PublicAPI] public class List: Element<ListView>
    {
        [NotNull] private HashSet<int> visibleChildren = new ();
        [NotNull] private readonly (IComponent child, Portal.Link link)[] content;
        private List<MutableValue<bool>> items;

        [NotNull]
        public static List V(IEnumerable<IComponent> content, params IManipulator[] manipulators) =>
            new(content, manipulators);
        
        public override void Dispose()
        {
            foreach (var item in items)
                item?.Dispose();
            items.Clear();
            visibleChildren.Clear();
            base.Dispose();
        }
        
        protected override void OnState(CompositionContext context)
        {
            base.OnState(context);

            for (var i = 0; i < content.Length; ++i)
            {
                context.SetNextEntryId(i+1);
                content[i].child.Recompose(context);
            }
        }

        protected override ListView PrepareElement(ListView target)
        {
            target.makeItem = MakeItem;
            target.destroyItem = DestroyItem;
            target.bindItem = BindItem;
            target.unbindItem = UnbindItem;

            target.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;
            
            if (!Equals(target.itemsSource, items))
                target.itemsSource = items;

            foreach (var childId in visibleChildren)
                content[childId].child.Render();
            
            return target;
        }
        
        private List(IEnumerable<IComponent> content, IManipulator[] manipulators) : base(manipulators)
        {
            this.content = content.Select(WrapChild).ToArray();
            items = Enumerable.Repeat<MutableValue<bool>>(null, this.content.Length).ToList();
            return;

            (IComponent, Portal.Link) WrapChild(IComponent contentElement, int i)
            {
                var link = new Portal.Link();

                var child = Comp(ctx =>
                {
                    var visible = ctx.Remember(false);

                    items[i] = visible;
                    
                    return Portal.V(contentElement, link);
                });
                
                return (child, link);
            }
        }

        private static VisualElement MakeItem() => new ();
        private static void DestroyItem(VisualElement item)
        {
            CompositionContext.ElementUserData.CleanUp(item);
            item.Clear();
        }

        private void BindItem(VisualElement item, int id)
        {
            if (items == null)
                return;

            if (id < 0 || items.Count <= id)
                return;

            var visible = items[id];
            
            if (visible == null)
                return;
            
            var link = content[id].link;
            link.Container = item;

            visibleChildren.Add(id);
            visible.Value = true;
        }

        private void UnbindItem(VisualElement item, int id)
        {
            if (items == null)
                return;

            if (id < 0 || items.Count <= id)
                return;

            var visible = items[id];
            
            if (visible == null)
                return;

            var link = content[id].link;
            link.Container = null;
            item.Clear();
            visibleChildren.Remove(id);
            
            visible.Value = false;
        }
    }
}