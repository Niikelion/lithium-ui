using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine.UIElements;
using System.Collections.Generic;

namespace UI.Li.Common
{
    public class Dropdown: Element
    {
        private readonly int initialValue;
        private readonly List<string> options;
        private readonly Action<int> onSelectionChanged;
        private MutableValue<int> currentValue;
        
        private WeakReference<CompositionContext> ctxRef;

        [PublicAPI]
        [NotNull]
        public static Dropdown V(
            int initialValue,
            [NotNull] Action<int> onSelectionChanged,
            [NotNull] List<string> options,
            Data data = new()
        ) => new(initialValue, onSelectionChanged, options, data);

        [PublicAPI]
        [NotNull]
        public static Dropdown V<T>(
            int initialValue,
            [NotNull] Action<T> onSelectionChanged,
            Data data = new()
        ) where T : Enum
        {
            var options = Enum.GetValues(typeof(T)).Cast<T>().ToArray();
            return new(initialValue, v => onSelectionChanged(options[v]), options.Select(o => o.ToString()).ToList(), data);
        }
        
        private Dropdown(int initialValue, Action<int> onSelectionChanged, List<string> options, Data data) : base(data)
        {
            this.initialValue = initialValue;
            this.onSelectionChanged = onSelectionChanged;
            this.options = options;
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
            var field = Use<DropdownField>(source);

            field.choices = options;

            int index = currentValue?.Value ?? 0;

            field.index = index >= 0 && index < options.Count ? index : 0;

            field.RegisterValueChangedCallback(OnSelectionChanged);
            
            AddCleanup(field, () => field.UnregisterValueChangedCallback(OnSelectionChanged));
            
            return field;
        }

        private void OnSelectionChanged(ChangeEvent<string> evt)
        {
            if (ctxRef == null || !ctxRef.TryGetTarget(out var ctx))
                return;

            if (evt.target is not DropdownField dropdown)
                return;

            int index = dropdown.index;

            using var _ = ctx.BatchOperations();

            currentValue.Value = index;
            onSelectionChanged?.Invoke(index);
        }
    }
}