using UI.Li.Utils;
using UnityEditor;

using static UI.Li.Editor.Fields;
using static UI.Li.Fields.Fields;

namespace UI.Li.Editor.Debugging
{
    [CustomPropertyDrawer(typeof(ValueReference<>), true)]
    public class ValueReferenceEditor: ComposablePropertyDrawer
    {
        protected override bool HideContext => true;
        protected override IComponent Layout(SerializedProperty property)
        {
            var prop = property.FindPropertyRelative("Value");

            if (prop == null)
                return TextField(_ => {}, property.boxedValue.ToString()).Manipulate(new Disabled());
            
            return Property(prop, label: "");
        }
    }
}