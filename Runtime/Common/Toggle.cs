﻿using JetBrains.Annotations;
using System;
using UnityEngine.UIElements;

namespace UI.Li.Common
{
    [PublicAPI] public sealed class Toggle: Element
    {
        [NotNull] private readonly Action<bool> onValueChanged;
        private readonly bool initialValue;

        [NotNull] [Obsolete]
        public static Toggle V([NotNull] Action<bool> onValueChanged, bool initialValue, Data data) =>
            new (onValueChanged, initialValue, data);
        
        [NotNull]
        public static Toggle V([NotNull] Action<bool> onValueChanged, bool initialValue = false, params IManipulator[] manipulators) =>
            new (onValueChanged, initialValue, manipulators);

        public override bool StateLayoutEquals(IComponent other) => other is Toggle;

        protected override VisualElement GetElement(VisualElement source)
        {
            var element = Use<UnityEngine.UIElements.Toggle>(source);
            
            element.value = initialValue;
            element.RegisterValueChangedCallback(OnValueChanged);
            AddCleanup(element, () => element.UnregisterValueChangedCallback(OnValueChanged));
            
            return element;
        }

        protected override VisualElement PrepareElement(VisualElement target)
        {
            var elem = base.PrepareElement(target);

            elem.AddToClassList(UnityEngine.UIElements.Toggle.ussClassName);
            elem.AddToClassList(BaseField<bool>.ussClassName);

            return elem;
        }

        private void OnValueChanged(ChangeEvent<bool> evt) => onValueChanged(evt.newValue);
        
        [Obsolete] private Toggle([NotNull] Action<bool> onValueChanged, bool initialValue, Data data): base(data)
        {
            this.onValueChanged = onValueChanged;
            this.initialValue = initialValue;
        }
        
        private Toggle([NotNull] Action<bool> onValueChanged, bool initialValue, IManipulator[] manipulators): base(manipulators)
        {
            this.onValueChanged = onValueChanged;
            this.initialValue = initialValue;
        }
    }
}