using UI.Li.Utils;
using UnityEditor;

using static UI.Li.Fields.Fields;

namespace UI.Li.Editor.Debugging
{
    [CustomPropertyDrawer(typeof(MutableList<>))]
    public class MutableListEditor: ComposablePropertyDrawer
    {
        protected override bool HideContext => true;

        protected override IComponent Layout(SerializedProperty property) =>
            TextField(_ => { }, property.boxedValue.ToString()).Manipulate(new Disabled());
    }
}