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
    [PublicAPI]
    public sealed class Dropdown: Element<DropdownField>
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
        /// <param name="manipulators">manipulators <seealso cref="IManipulator"/></param>
        /// <returns></returns>
        [NotNull]
        public static Dropdown V(
            int initialValue,
            [NotNull] Action<int> onSelectionChanged,
            [NotNull] List<string> options,
            params IManipulator[] manipulators
        ) => new(initialValue, onSelectionChanged, options, manipulators);

        /// <summary>
        /// Creates <see cref="Dropdown"/> instance using enum.
        /// </summary>
        /// <param name="initialValue">default selected option</param>
        /// <param name="onSelectionChanged">callback invoked every time selected option changes</param>
        /// <param name="manipulators">manipulators <seealso cref="IManipulator"/></param>
        /// <typeparam name="T">enum to be displayed in dropdown</typeparam>
        /// <returns></returns>
        [NotNull]
        public static Dropdown V<T>(
            T initialValue,
            [NotNull] Action<T> onSelectionChanged,
            params IManipulator[] manipulators
        ) where T : Enum
        {
            var options = Enum.GetValues(typeof(T)).Cast<T>().ToList();
            var stringOptions = options.Select(o => o.ToString()).ToList();
            return new(options.IndexOf(initialValue), v => onSelectionChanged(options[v]), stringOptions, manipulators);
        }
        
        public override void Dispose()
        {
            currentValue = null;
            ctxRef = null;
            base.Dispose();
        }

        public override string ToString() => $"{base.ToString()} [{options[currentValue?.Value ?? initialValue]}]";

        public override bool StateLayoutEquals(IComponent other) =>
            other is Dropdown dropdown && options.SequenceEqual(dropdown.options);
        
        private Dropdown(int initialValue, Action<int> onSelectionChanged, List<string> options, IManipulator[] manipulators) : base(manipulators)
        {
            this.initialValue = initialValue;
            this.onSelectionChanged = onSelectionChanged;
            this.options = options;
        }

        protected override void OnState(CompositionContext context)
        {
            ctxRef = new (context);
            currentValue = context.Remember(initialValue);
        }

        protected override DropdownField PrepareElement(DropdownField element)
        {
            
            element.AddToClassList(DropdownField.ussClassName);
            element.AddToClassList(PopupField<string>.ussClassName);
            element.AddToClassList(BasePopupField<string, string>.ussClassName);
            element.AddToClassList(BaseField<string>.ussClassName);
            
            int index = currentValue?.Value ?? 0;
            
            element.choices = options;
            element.index = index >= 0 && index < options.Count ? index : 0;

            element.RegisterValueChangedCallback(OnSelectionChanged);
            AddCleanup(element, () => element.UnregisterValueChangedCallback(OnSelectionChanged));
            
            return element;
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