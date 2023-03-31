using UnityEditor;
using UI.Li.Common;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace UI.Li.Editor
{
    [PublicAPI]
    public class CustomField : Element
    {
        private readonly IComponent editor;
        private readonly string name;

        [PublicAPI]
        [NotNull]
        public static CustomField V(SerializedProperty property, IComponent editor, Data data = new())
            => new(property, editor, data);

        private CustomField(SerializedProperty property, IComponent editor, Data data): base(data)
        {
            this.editor = editor;
            name = property.displayName;
        }

        public override void Dispose()
        {
            editor.Dispose();
            base.Dispose();
        }

        protected override void OnState(CompositionContext context) => editor.Recompose(context);

        protected override VisualElement PrepareElement(VisualElement target)
        {
            var ret = base.PrepareElement(target);

            ret.Clear();
            ret.AddToClassList("unity-base-field");
            
            var label = new Label(name);
            label.AddToClassList("unity-base-field__label");

            var content = new VisualElement();
            content.AddToClassList("unity-base-field__input");
            content.Add(editor.Render());

            ret.style.justifyContent = Justify.SpaceBetween;
            ret.RegisterCallback<GeometryChangedEvent>(_ =>
            {
                var panel = ret.panel;
                var size = panel.visualTree.contentRect.size;
                
                // magic value measured with ruler, may change in the future!
                content.style.maxWidth = size.x * 0.55f + 10;
            });
            
            ret.Add(label);
            ret.Add(content);
            
            return ret;
        }
    }
}