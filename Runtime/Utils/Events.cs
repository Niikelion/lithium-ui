using System;
using JetBrains.Annotations;
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
        public static IManipulator OnKey(Action<KeyHandler.SyntheticKeyEvent> onKeyDown = null,
            Action<KeyHandler.SyntheticKeyEvent> onKeyUp = null) => new KeyHandler(onKeyDown, onKeyUp);

        [NotNull]
        public static IManipulator OnBlur([NotNull] Action onBlur) =>
            new Blurrable(onBlur);
        
        [NotNull]
        public static IManipulator OnFocus([NotNull] Action onFocus) =>
            new Focusable(onFocus);
    }

    [PublicAPI]
    public class Repaintable : Manipulator
    {
        [NotNull] private readonly Action<MeshGenerationContext> onRepaint;
        
        public Repaintable([NotNull] Action<MeshGenerationContext> onRepaint) => this.onRepaint = onRepaint;

        protected override void RegisterCallbacksOnTarget() => target.generateVisualContent += onRepaint;

        protected override void UnregisterCallbacksFromTarget() => target.generateVisualContent -= onRepaint;
    }

    [PublicAPI]
    public class KeyHandler : Manipulator
    {
        /// <summary>
        /// Simplified <see cref="KeyboardEventBase{T}"/>, <see cref="KeyDownEvent"/> and <see cref="KeyUpEvent"/>.
        /// </summary>
        /// <remarks>Exposes only important event data and omits all references to objects from UIElements system to improve encapsulation.</remarks>
        public struct SyntheticKeyEvent
        {
            public enum EventType
            {
                Up,
                Down
            }
            
            [PublicAPI] public readonly char Character;
            [PublicAPI] public readonly bool Alt;
            [PublicAPI] public readonly bool Ctrl;
            [PublicAPI] public readonly bool Shift;
            [PublicAPI] public readonly bool Cmd;
            [PublicAPI] public readonly EventType Type;

            public SyntheticKeyEvent(EventType type, char character, bool alt, bool ctrl, bool shift, bool cmd)
            {
                Type = type;
                Character = character;
                Alt = alt;
                Ctrl = ctrl;
                Shift = shift;
                Cmd = cmd;
            }
        }

        public KeyHandler(Action<SyntheticKeyEvent> onKeyDown = null, Action<SyntheticKeyEvent> onKeyUp = null)
        {
            this.onKeyDown = onKeyDown;
            this.onKeyUp = onKeyUp;
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

        private void OnKeyDown(KeyDownEvent e) => onKeyDown?.Invoke(new(
            type: SyntheticKeyEvent.EventType.Down,
            character: e.character,
            alt: e.altKey,
            ctrl: e.ctrlKey,
            shift: e.shiftKey,
            cmd: e.commandKey
            ));
        
        private void OnKeyUp(KeyUpEvent e) => onKeyDown?.Invoke(new(
            type: SyntheticKeyEvent.EventType.Up,
            character: e.character,
            alt: e.altKey,
            ctrl: e.ctrlKey,
            shift: e.shiftKey,
            cmd: e.commandKey
        ));
    }

    public class ActionHandlerBase<T> : Manipulator where T : EventBase<T>, new()
    {
        [NotNull] private readonly Action handler;

        protected ActionHandlerBase(Action handler) => this.handler = handler;

        protected override void RegisterCallbacksOnTarget() => target.RegisterCallback<T>(OnEvent);
        protected override void UnregisterCallbacksFromTarget() => target.UnregisterCallback<T>(OnEvent);
        private void OnEvent(T _) => handler();
    }
    
    [PublicAPI]
    public class Blurrable : ActionHandlerBase<BlurEvent>
    {
        public Blurrable([NotNull] Action onBlur) : base(onBlur) { }
    }
    
    [PublicAPI]
    public class Focusable : ActionHandlerBase<FocusEvent>
    {
        public Focusable([NotNull] Action onFocus) : base(onFocus) { }
    }
}