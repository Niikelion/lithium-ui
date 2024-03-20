using System;
using UnityEditor;
using UI.Li.Common;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace UI.Li.Editor
{
    public class CustomFieldView : VisualElement
    {
        [PublicAPI]
        public string text
        {
            get => labelValue;
            set
            {
                labelValue = value;
                labelView.text = labelValue;
            }
        }

        public override VisualElement contentContainer => contentView ?? base.contentContainer;
        private string labelValue;
        private readonly Label labelView;
        private readonly VisualElement contentView;

        [PublicAPI]
        public CustomFieldView(): this("") {}

        [PublicAPI]
        public CustomFieldView(string value)
        {
            labelValue = value;
            
            AddClasses();
            
            labelView = new (labelValue);
            labelView.AddToClassList("unity-base-field__label");

            var content = new VisualElement();
            content.AddToClassList("unity-base-field__input");
            content.style.flexGrow = 1;
            content.style.flexShrink = 1;

            style.justifyContent = Justify.SpaceBetween;
            RegisterCallback<GeometryChangedEvent>(_ =>
            {
                var size = panel.visualTree.contentRect.size;
                
                // magic value measured with ruler, may change in the future!
                labelView.style.width = size.x * 0.45f-40.47f;
            });
            
            Add(labelView);
            Add(content);
            contentView = content;
        }

        public void AddClasses()
        {
            AddToClassList("unity-base-field");
            AddToClassList("unity-base-field__aligned");
        }
    }
    
    [PublicAPI]
    public class CustomField: Element
    {
        private readonly IComponent editor;
        private readonly string name;

        [NotNull] [Obsolete]
        public static CustomField V(SerializedProperty property, IComponent editor, Data data)
            => new(property, editor, data);
        
        [NotNull]
        public static CustomField V(SerializedProperty property, IComponent editor, params IManipulator[] manipulators)
            => new(property, editor, manipulators);

        public override void Dispose()
        {
            editor.Dispose();
            base.Dispose();
        }

        protected override void OnState(CompositionContext context) => editor.Recompose(context);

        protected override VisualElement PrepareElement(VisualElement target)
        {
            var ret = base.PrepareElement(target);
            ret.Add(editor.Render());

            if (ret is CustomFieldView view)
                view.AddClasses();
            
            return ret;
        }

        protected override VisualElement GetElement(VisualElement source)
        {
            var element = Use<CustomFieldView>(source);
            element.text = name;
            
            return element;
        }
        
        [Obsolete] private CustomField(SerializedProperty property, IComponent editor, Data data): base(data)
        {
            this.editor = editor;
            name = property.displayName;
        }
        
        private CustomField(SerializedProperty property, IComponent editor, IManipulator[] manipulators): base(manipulators)
        {
            this.editor = editor;
            name = property.displayName;
        }
    }
}