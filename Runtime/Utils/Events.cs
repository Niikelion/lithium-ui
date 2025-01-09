using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Li.Utils
{
    /// <summary>
    /// Utility class aimed to simplify basic use of manipulators.
    /// </summary>
    [PublicAPI]
    public static class EventUtils
    {
        [NotNull]
        public static IManipulator OnClick([NotNull] Action onClick) =>
            new Clickable(onClick);

        [NotNull]
        public static IManipulator OnRepaint([NotNull] Action<MeshGenerationContext> onRepaint) =>
            new Repaintable(onRepaint);

        [NotNull]
        public static IManipulator OnKey(Action<SyntheticKeyEvent> onKeyDown = null,
            Action<SyntheticKeyEvent> onKeyUp = null) => new KeyHandler(onKeyDown, onKeyUp);

        [NotNull]
        public static IManipulator OnBlur([NotNull] Action onBlur) =>
            new Blurrable(onBlur);
        
        [NotNull]
        public static IManipulator OnFocus([NotNull] Action onFocus) =>
            new Focusable(onFocus);

        [NotNull]
        public static IManipulator OnFocusOut([NotNull] Action onFocusOut) =>
            new FocusOutHandler(onFocusOut);
        
        [NotNull]
        public static IManipulator OnFocusIn([NotNull] Action onFocusIn) =>
            new FocusOutHandler(onFocusIn);
        
        [NotNull]
        public static IManipulator OnMouseEnter([NotNull] Action<SyntheticMouseEvent> onMouseEnter) =>
            new MouseEnterHandler(onMouseEnter);

        [NotNull]
        public static IManipulator OnMouseLeave([NotNull] Action<SyntheticMouseEvent> onMouseLeave) =>
            new MouseLeaveHandler(onMouseLeave);

        [NotNull]
        public static IManipulator OnSize([NotNull] Action<Rect> onSizeChanged) =>
            new SizeWatcher(onSizeChanged);
        
        [NotNull]
        public static Action WrapEventHandler([NotNull] Action handler)
        {
            var defer = ComponentState.GetDeferrer();
            return () => defer(handler);
        }

        [NotNull]
        public static Action<T> WrapEventHandler<T>([NotNull] Action<T> handler)
        {
            var defer = ComponentState.GetDeferrer();
            return e => defer(() => handler(e));
        }
    }

    [PublicAPI]
    public class Repaintable : Manipulator
    {
        [NotNull] private readonly Action<MeshGenerationContext> onRepaint;
        
        public Repaintable([NotNull] Action<MeshGenerationContext> onRepaint) => this.onRepaint = EventUtils.WrapEventHandler(onRepaint);

        protected override void RegisterCallbacksOnTarget() => target.generateVisualContent += onRepaint;

        protected override void UnregisterCallbacksFromTarget() => target.generateVisualContent -= onRepaint;
    }

    /// <summary>
    /// Simplified <see cref="KeyboardEventBase{T}"/>, <see cref="KeyDownEvent"/> and <see cref="KeyUpEvent"/>.
    /// </summary>
    /// <remarks>Exposes only important event data and omits all references to objects from UIElements system to improve encapsulation.</remarks>
    [PublicAPI] public struct SyntheticKeyEvent
    {
        public enum EventType
        {
            Up,
            Down
        }
            
        public readonly char Character => sourceEvent.character;
        public readonly bool Alt => sourceEvent.altKey;
        public readonly bool Ctrl => sourceEvent.ctrlKey;
        public readonly bool Shift => sourceEvent.shiftKey;
        public readonly bool Cmd => sourceEvent.commandKey;
        public readonly EventType Type;

        private IKeyboardEvent sourceEvent;
            
        public SyntheticKeyEvent(EventType type, IKeyboardEvent source)
        {
            Type = type;
            sourceEvent = source;
        }
    }

    [PublicAPI]
    public struct SyntheticMouseEvent
    {
        public enum EventType
        {
            Enter,
            Leave
        }

        public readonly EventType Type;
        private readonly IMouseEvent sourceEvent;

        public SyntheticMouseEvent(EventType type, IMouseEvent source)
        {
            Type = type;
            sourceEvent = source;
        }
    }
    
    [PublicAPI]
    public sealed class KeyHandler : Manipulator
    {
        public KeyHandler(Action<SyntheticKeyEvent> onKeyDown = null, Action<SyntheticKeyEvent> onKeyUp = null)
        {
            this.onKeyDown = EventUtils.WrapEventHandler(onKeyDown);
            this.onKeyUp = EventUtils.WrapEventHandler(onKeyUp);
        }
        
        private readonly Action<SyntheticKeyEvent> onKeyDown, onKeyUp;
        
        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<KeyDownEvent>(OnKeyDown);
            target.RegisterCallback<KeyUpEvent>(OnKeyUp);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<KeyDownEvent>(OnKeyDown);
            target.UnregisterCallback<KeyUpEvent>(OnKeyUp);
        }

        private void OnKeyDown(KeyDownEvent e) =>
            onKeyDown?.Invoke(new (type: SyntheticKeyEvent.EventType.Down, e));
        
        private void OnKeyUp(KeyUpEvent e) =>
            onKeyDown?.Invoke(new (type: SyntheticKeyEvent.EventType.Up, e));
    }

    public abstract class ActionHandlerBase<T> : Manipulator where T : EventBase<T>, new()
    {
        [NotNull] private readonly Action handler;

        protected ActionHandlerBase([NotNull] Action handler) => this.handler = EventUtils.WrapEventHandler(handler);

        protected override void RegisterCallbacksOnTarget() => target.RegisterCallback<T>(OnEvent);
        protected override void UnregisterCallbacksFromTarget() => target.UnregisterCallback<T>(OnEvent);
        private void OnEvent(T _) => handler();
    }
    
    [PublicAPI]
    public sealed class Blurrable : ActionHandlerBase<BlurEvent>
    {
        public Blurrable([NotNull] Action onBlur) : base(onBlur) { }
    }
    
    [PublicAPI]
    public sealed class Focusable : ActionHandlerBase<FocusEvent>
    {
        public Focusable([NotNull] Action onFocus) : base(onFocus) { }
    }

    [PublicAPI]
    public sealed class FocusOutHandler : ActionHandlerBase<FocusOutEvent>
    {
        public FocusOutHandler([NotNull] Action onFocusOut) : base(onFocusOut) { }
    }

    [PublicAPI]
    public sealed class FocusInHandler : ActionHandlerBase<FocusInEvent>
    {
        public FocusInHandler([NotNull] Action onFocusIn) : base(onFocusIn) { }
    }

    [PublicAPI]
    public abstract class MouseHandlerBase<T> : Manipulator where T : MouseEventBase<T>, new()
    {
        [NotNull] private readonly Action<SyntheticMouseEvent> onMouseEvent;
        private readonly SyntheticMouseEvent.EventType eventType;
        
        protected MouseHandlerBase([NotNull] Action<SyntheticMouseEvent> onMouseEvent, SyntheticMouseEvent.EventType eventType)
        {
            this.onMouseEvent = EventUtils.WrapEventHandler(onMouseEvent);
            this.eventType = eventType;
        }

        protected override void RegisterCallbacksOnTarget() => target.RegisterCallback<T>(OnMouseEvent);

        protected override void UnregisterCallbacksFromTarget() => target.UnregisterCallback<T>(OnMouseEvent);
        
        private void OnMouseEvent(T e) => onMouseEvent(new (eventType, e));
    }

    [PublicAPI]
    public sealed class MouseEnterHandler : MouseHandlerBase<MouseEnterEvent>
    {
        public MouseEnterHandler([NotNull] Action<SyntheticMouseEvent> onMouseEnter) : base(onMouseEnter, SyntheticMouseEvent.EventType.Enter) { }
    }
    
    [PublicAPI]
    public sealed class MouseLeaveHandler : MouseHandlerBase<MouseEnterEvent>
    {
        public MouseLeaveHandler([NotNull] Action<SyntheticMouseEvent> onMouseLeave) : base(onMouseLeave, SyntheticMouseEvent.EventType.Leave) { }
    }
    
    [PublicAPI]
    public class SizeWatcher : Manipulator
    {
        [NotNull] private readonly Action<Rect> onSizeChanged;

        public SizeWatcher([NotNull] Action<Rect> onSizeChanged) => this.onSizeChanged = EventUtils.WrapEventHandler(onSizeChanged);
        
        protected override void RegisterCallbacksOnTarget() =>
            target.RegisterCallback<GeometryChangedEvent>(OnSizeChanged);

        protected override void UnregisterCallbacksFromTarget() =>
            target.UnregisterCallback<GeometryChangedEvent>(OnSizeChanged);

        private void OnSizeChanged(GeometryChangedEvent evt) =>
            onSizeChanged.Invoke(evt.newRect);
    }
}