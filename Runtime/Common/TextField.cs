using System;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace UI.Li.Common
{
    /// <summary>
    /// Component representing <see cref="UnityEngine.UIElements.TextField"/>.
    /// </summary>
    [PublicAPI] public class TextField: Element
    {
        // TODO: show tooltip as grayed-out text when empty
        private readonly string tooltip;
        private readonly string initialValue;
        private readonly bool focused;
        private MutableValue<string> currentValue;
        private readonly Action<string> onValueChanged;
        
        private WeakReference<CompositionContext> ctxRef;

        /// <summary>
        /// Constructs <see cref="TextField"/> instance.
        /// </summary>
        /// <param name="onValueChanged">callback invoked every time value is changed</param>
        /// <param name="value">initial value</param>
        /// <param name="tooltip">tooltip</param>
        /// <param name="focused">indicates whether or not element should be focused after render, currently does nothing</param>
        /// <param name="data">additional data <seealso cref="Element.Data"/></param>
        /// <returns></returns>
        [PublicAPI]
        [NotNull]
        public static TextField V(
            [NotNull] Action<string> onValueChanged,
            [NotNull] string value = "",
            [NotNull] string tooltip = "",
            bool focused = false,
            Data data = new()
        ) => new(onValueChanged, value, tooltip, focused, data);
        
        private TextField(Action<string> onValueChanged, string value, string tooltip, bool focused, Data data): base(data)
        {
            this.onValueChanged = onValueChanged;
            
            initialValue = value;

            this.tooltip = tooltip;
            this.focused = focused;
        }

        public override void Dispose()
        {
            currentValue = null;
            ctxRef = null;
            base.Dispose();
        }

        protected override void OnState(CompositionContext context)
        {
            ctxRef = new WeakReference<CompositionContext>(context);
            currentValue = context.Remember(initialValue);
        }

        protected override VisualElement GetElement(VisualElement source)
        {
            var field = Use<UnityEngine.UIElements.TextField>(source);

            field.tooltip = tooltip;
            field.value = currentValue;

            field.RegisterValueChangedCallback(OnValueChanged);
            
            AddCleanup(field, () => field.UnregisterValueChangedCallback(OnValueChanged));

            return field;
        }

        protected override VisualElement PrepareElement(VisualElement target)
        {
            var ret = base.PrepareElement(target);

            ret.AddToClassList("unity-base-field");
            ret.AddToClassList("unity-base-text-field");
            ret.AddToClassList("unity-text-field");

            return ret;
        }

        private void OnValueChanged(ChangeEvent<string> evt)
        {
            if (evt.newValue == evt.previousValue)
                return;

            if (!ctxRef.TryGetTarget(out var ctx))
                return;

            string v = evt.newValue;

            using var _ = ctx.BatchOperations();
            
            currentValue.Value = v;
            onValueChanged?.Invoke(v);
        }
    }
}