using System;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine.UIElements;
using System.Collections.Generic;

namespace UI.Li.Common
{
    /// <summary>
    /// Component representing <see cref="DropdownField"/>.
    /// </summary>
    public class Dropdown: Element
    {
        private readonly int initialValue;
        private readonly List<string> options;
        private readonly Action<int> onSelectionChanged;
        private MutableValue<int> currentValue;
        
        private WeakReference<CompositionContext> ctxRef;

        /// <summary>
        /// Creates <see cref="Dropdown"/> instance using list of options.
        /// </summary>
        /// <param name="initialValue">default selected option</param>
        /// <param name="onSelectionChanged">callback invoked every time selected option changes</param>
        /// <param name="options">available options for dropdown</param>
        /// <param name="data">additional element data <seealso cref="Element.Data"/></param>
        /// <returns></returns>
        [PublicAPI]
        [NotNull]
        public static Dropdown V(
            int initialValue,
            [NotNull] Action<int> onSelectionChanged,
            [NotNull] List<string> options,
            Data data = new()
        ) => new(initialValue, onSelectionChanged, options, data);

        /// <summary>
        /// Creates <see cref="Dropdown"/> instance using enum.
        /// </summary>
        /// <param name="initialValue">default selected option</param>
        /// <param name="onSelectionChanged">callback invoked every time selected opton changes</param>
        /// <param name="data">additional element data <seealso cref="Element.Data"/></param>
        /// <typeparam name="T">enum to be displayed in dropdown</typeparam>
        /// <returns></returns>
        [PublicAPI]
        [NotNull]
        public static Dropdown V<T>(
            T initialValue,
            [NotNull] Action<T> onSelectionChanged,
            Data data = new()
        ) where T : Enum
        {
            var options = Enum.GetValues(typeof(T)).Cast<T>().ToList();
            return new(options.IndexOf(initialValue), v => onSelectionChanged(options[v]), options.Select(o => o.ToString()).ToList(), data);
        }
        
        public override void Dispose()
        {
            currentValue = null;
            ctxRef = null;
            base.Dispose();
        }
        
        private Dropdown(int initialValue, Action<int> onSelectionChanged, List<string> options, Data data) : base(data)
        {
            this.initialValue = initialValue;
            this.onSelectionChanged = onSelectionChanged;
            this.options = options;
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