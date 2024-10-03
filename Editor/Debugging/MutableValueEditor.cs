using System.Reflection;
using JetBrains.Annotations;
using UnityEditor;

using static UI.Li.ComponentState;

namespace UI.Li.Editor.Debugging
{
    [CustomPropertyDrawer(typeof(MutableValue<>), true)]
    public class MutableValueEditor: ComposablePropertyDrawer
    {
        protected override bool HideContext => true;

        protected override IComponent Layout(SerializedProperty property) => WithState(() =>
        {
            var initialized = RememberRef(false);
            var lastValue = RememberRef<object>(null);
            
            return PropertyField.V(property.FindPropertyRelative("value"),
                onValueChanged: v =>
                {
                    if (!initialized.Value)
                    {
                        lastValue.Value = v;
                        initialized.Value = true;
                        return;
                    }

                    if (lastValue.Value.Equals(v))
                        return;

                    lastValue.Value = v;
                    
                    UpdateGenericValue(property, v);
                });
        });

        private static void UpdateGenericValue(SerializedProperty property, object newValue)
        {
            var target = property.managedReferenceValue;
            if (target == null)
                return;

            var t = target.GetType();

            if (!t.IsGenericType)
                return;
            
            var tArgs = t.GetGenericArguments();

            if (tArgs.Length != 1)
                return;

            if (!typeof(MutableValue<>).MakeGenericType(tArgs).IsAssignableFrom(t))
                return;
            
            var method = typeof(MutableValueEditor).GetMethod("UpdateValue", BindingFlags.NonPublic | BindingFlags.Static)!.MakeGenericMethod(tArgs);
            method.Invoke(null, new [] { property, newValue });
        }
        
        [UsedImplicitly] private static void UpdateValue<T>(SerializedProperty property, object newValue)
        {
            if (property.managedReferenceValue is not MutableValue<T> value)
                return;

            value.Value = (T)newValue;
        }
    }
}