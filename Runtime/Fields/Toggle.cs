using JetBrains.Annotations;
using System;
using UnityEngine.UIElements;

namespace UI.Li.Fields
{
    [PublicAPI] public sealed class Toggle: FieldBase<bool, UnityEngine.UIElements.Toggle>
    {
        [NotNull]
        public static Toggle V([NotNull] Action<bool> onValueChanged, bool initialValue = false, params IManipulator[] manipulators) =>
            new (onValueChanged, initialValue, manipulators);

        public override bool StateLayoutEquals(IComponent other) => other is Toggle;

        protected override UnityEngine.UIElements.Toggle PrepareElement(UnityEngine.UIElements.Toggle target)
        {
            var elem = base.PrepareElement(target);
            return elem;
        }
        
        private Toggle([NotNull] Action<bool> onValueChanged, bool initialValue, IManipulator[] manipulators): base(onValueChanged, initialValue, manipulators) { }
    }
}