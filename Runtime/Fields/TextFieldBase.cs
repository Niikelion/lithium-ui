using System;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace UI.Li.Fields
{
    public abstract class TextFieldBase<TValue, TElem>: FieldBase<TValue, TElem> where TElem : TextInputBaseField<TValue>, new()
    {
        [NotNull] private readonly string placeholder;
        private readonly bool focus;
        
        [CanBeNull] private ValueReference<bool> focused;
        
        public override void Dispose()
        {
            focused?.Dispose();
            base.Dispose();
        }
        
        protected TextFieldBase([NotNull] Action<TValue> onValueChanged, TValue initialValue, [NotNull] string placeholder, bool focus, IManipulator[] manipulators) : base(onValueChanged, initialValue, manipulators)
        {
            this.focus = focus;
            this.placeholder = placeholder;
        }

        protected override void OnState(CompositionContext context)
        {
            base.OnState(context);
            focused = context.RememberRef(false);
        }

        protected override TElem PrepareElement(TElem target)
        {
            var elem = base.PrepareElement(target);

            elem.textEdition.placeholder = placeholder;
            elem.textEdition.hidePlaceholderOnFocus = false;
            
            HandleFocus(elem);
            
            return elem;
        }

        private void HandleFocus([NotNull] VisualElement element)
        {
            if (focused == null || focused || !focus) return;
            
            focused.Value = true;
            element.schedule.Execute(element.Focus);
        }
    }
}