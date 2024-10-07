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

        [NotNull]
        public static DefaultInspector V([NotNull] UnityEditor.Editor editor, params IManipulator[] manipulators) =>
            new (editor, manipulators);

        protected override VisualElement GetElement(VisualElement source) =>
            Use<VisualElement>(source);

        protected override VisualElement PrepareElement(VisualElement target)
        {
            var element = base.PrepareElement(target);

            InspectorElement.FillDefaultInspector(element, editor.serializedObject, editor);
            
            return element;
        }

        private DefaultInspector([NotNull] UnityEditor.Editor editor, IManipulator[] manipulators): base(manipulators) => this.editor = editor;
    }
}