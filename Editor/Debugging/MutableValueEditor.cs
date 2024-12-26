using System;
using System.Reflection;
using UI.Li.Utils;
using UnityEditor;
using UnityEngine;

using static UI.Li.Common.Common;
using static UI.Li.Fields.Fields;
using static UI.Li.Editor.Fields;
using static UI.Li.Utils.EventUtils;

namespace UI.Li.Editor.Debugging
{
    [CustomPropertyDrawer(typeof(MutableValue<>), true)]
    public class MutableValueEditor: ComposablePropertyDrawer
    {
        protected override bool HideContext => true;
        protected override IComponent Layout(SerializedProperty property)
        {
            var prop = property.FindPropertyRelative("value");

            var obj = property.boxedValue;
            
            if (prop == null)
                return  TextField(_ => {}, obj.ToString()).Manipulate(new Disabled());

            var type = obj.GetType();
            var notify = type.GetMethod("NotifyChanged", BindingFlags.Instance | BindingFlags.NonPublic);
            Debug.Assert(notify != null);
            
            return Property(prop, label: "").Manipulate(OnFocusOut(() => notify.Invoke(obj, Array.Empty<object>())));
        }
    }
}