using JetBrains.Annotations;
using UI.Li.Common;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace UI.Li.Editor
{
    [PublicAPI]
    public class DefaultInspector: Element
    {
        [NotNull] private readonly UnityEditor.Editor editor;
        
        [NotNull] public static DefaultInspector V([NotNull] UnityEditor.Editor editor, Data data = new()) => new (editor, data);
        
        private DefaultInspector([NotNull] UnityEditor.Editor editor, Data data): base(data)
        {
            this.editor = editor;
        }

        protected override VisualElement GetElement(VisualElement source)
        {
            var element = new VisualElement();
            
            InspectorElement.FillDefaultInspector(element, editor.serializedObject, editor);
            
            return element;
        }
    }
}