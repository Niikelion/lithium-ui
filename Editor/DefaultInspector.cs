using System;
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
        
        [NotNull] [Obsolete]
        public static DefaultInspector V([NotNull] UnityEditor.Editor editor, Data data) =>
            new (editor, data);

        [NotNull]
        public static DefaultInspector V([NotNull] UnityEditor.Editor editor, params IManipulator[] manipulators) =>
            new (editor, manipulators);

        protected override VisualElement GetElement(VisualElement source)
        {
            var element = new VisualElement();
            
            InspectorElement.FillDefaultInspector(element, editor.serializedObject, editor);
            
            return element;
        }
        
        [Obsolete] private DefaultInspector([NotNull] UnityEditor.Editor editor, Data data): base(data) => this.editor = editor;

        private DefaultInspector([NotNull] UnityEditor.Editor editor, IManipulator[] manipulators): base(manipulators) => this.editor = editor;
    }
}