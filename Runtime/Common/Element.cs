using System;
using JetBrains.Annotations;
using UnityEngine.UIElements;
using UI.Li.Utils.Continuations;
using System.Collections.Generic;

namespace UI.Li.Common
{
    /// <summary>
    /// Composition representing <see cref="VisualElement"/> without children.
    /// </summary>
    /// <remarks>Good base class for compositions that supports styling and callbacks of <see cref="VisualElement"/></remarks>
    [PublicAPI] public class Element: IComposition
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
        
        /// <summary>
        /// Struct containing all styles and events that can be applied to element.
        /// </summary>
        [PublicAPI] public struct Data
        {
            /// <summary>
            /// Structure used to store native event handlers.
            /// </summary>
            public struct Events
            {
                public EventCallback<ClickEvent> OnClick;
                public EventCallback<KeyDownEvent> OnKeyDown;
                public EventCallback<KeyUpEvent> OnKeyUp;
                public EventCallback<BlurEvent> OnBlur;
                public EventCallback<FocusEvent> OnFocus;

                /// <summary>
                /// Registers every non-null event handler for given element.
                /// </summary>
                public readonly void Register(VisualElement element)
                {
                    void Attach<T>(EventCallback<T> callback) where T : EventBase<T>, new() =>
                        element.RegisterCallback(callback);

                    OnClick?.Run(Attach);
                    OnKeyDown?.Run(Attach);
                    OnKeyUp?.Run(Attach);
                    OnBlur?.Run(Attach);
                    OnFocus?.Run(Attach);
                }
                
                /// <summary>
                /// Unregisters every non-null event handler for given element.
                /// </summary>
                public readonly void Unregister(VisualElement element)
                {
                    void Detach<T>(EventCallback<T> callback) where T : EventBase<T>, new() =>
                        element.UnregisterCallback(callback);
                    
                    OnClick?.Run(Detach);
                    OnKeyDown?.Run(Detach);
                    OnKeyUp?.Run(Detach);
                    OnBlur?.Run(Detach);
                    OnFocus?.Run(Detach);
                }
            }
            
            //  TODO: document
            public struct Frame
            {
                public readonly StyleLength? Left, Right, Top, Bottom;

                public Frame(StyleLength? left = null, StyleLength? right = null, StyleLength? top = null,
                    StyleLength? bottom = null)
                {
                    Left = left;
                    Right = right;
                    Top = top;
                    Bottom = bottom;
                }

                public Frame(StyleLength? horizontal = null, StyleLength? vertical = null) : this(
                    left: horizontal,
                    right: horizontal, 
                    top: vertical,
                    bottom: vertical
                ) { }
            }
            
            private readonly string name;
            private readonly List<string> classList;
            
            // TODO: maybe implement smth like react styled-components?
            #region styles
            private readonly DisplayStyle? display;
            private readonly FlexDirection? flexDirection;
            private readonly StyleFloat? flexShrink;
            private readonly StyleFloat? flexGrow;
            private readonly StyleLength? flexBasis;
            private readonly Align? alignItems;
            private readonly Align? alignContent;
            private readonly Justify? justifyContent;
            private readonly StyleLength? width;
            private readonly StyleLength? height;
            private readonly StyleLength? minWidth;
            private readonly StyleLength? minHeight;
            private readonly StyleLength? maxWidth;
            private readonly StyleLength? maxHeight;
            private readonly StyleColor? color;
            private readonly StyleColor? backgroundColor;
            private readonly StyleBackground? backgroundImage;
            private readonly Frame padding;
            private readonly Frame margin;
            #endregion

            #region callbacks
            private readonly Action onClick;
            private readonly Action<SyntheticKeyEvent> onKeyDown;
            private readonly Action<SyntheticKeyEvent> onKeyUp;
            private readonly Action onBlur;
            private readonly Action onFocus;
            #endregion

            [PublicAPI] public Data(
                [NotNull] string name = "",
                List<string> classList = null,
                DisplayStyle? display = null,
                FlexDirection? flexDirection = null,
                StyleFloat? flexShrink = null,
                StyleFloat? flexGrow = null,
                StyleLength? flexBasis = null,
                Align? alignItems = null,
                Align? alignContent = null,
                Justify? justifyContent = null,
                StyleLength? width = null,
                StyleLength? height = null,
                StyleLength? minWidth = null,
                StyleLength? minHeight = null,
                StyleLength? maxWidth = null,
                StyleLength? maxHeight = null,
                StyleColor? color = null,
                StyleColor? backgroundColor = null,
                StyleBackground? backgroundImage = null,
                Action onClick = null,
                Action<SyntheticKeyEvent> onKeyDown = null,
                Action<SyntheticKeyEvent> onKeyUp = null,
                Action onBlur = null,
                Action onFocus = null,
                Frame padding = new(),
                Frame margin = new()
            )
            {
                this.name = name;
                this.classList = classList;
                this.display = display;
                this.flexDirection = flexDirection;
                this.flexShrink = flexShrink;
                this.flexGrow = flexGrow;
                this.flexBasis = flexBasis;
                this.alignItems = alignItems;
                this.alignContent = alignContent;
                this.justifyContent = justifyContent;
                this.width = width;
                this.height = height;
                this.minWidth = minWidth;
                this.minHeight = minHeight;
                this.maxWidth = maxWidth;
                this.maxHeight = maxHeight;
                this.color = color;
                this.backgroundColor = backgroundColor;
                this.backgroundImage = backgroundImage;
                this.onClick = onClick;
                this.onKeyDown = onKeyDown;
                this.onKeyUp = onKeyUp;
                this.onBlur = onBlur;
                this.onFocus = onFocus;
                this.padding = padding;
                this.margin = margin;
            }

            public readonly Events Apply(VisualElement element)
            {
                element.name = name;
                
                element.ClearClassList();
                
                if (classList != null)
                    foreach (string cls in classList)
                        element.AddToClassList(cls);

                element.style.display = display?.Let(v => new StyleEnum<DisplayStyle>(v)) ?? StyleKeyword.Null;
                
                element.style.flexDirection = flexDirection?.Let(v => new StyleEnum<FlexDirection>(v)) ?? StyleKeyword.Null;

                element.style.flexShrink = flexShrink ?? StyleKeyword.Null;
                element.style.flexGrow = flexGrow ?? StyleKeyword.Null;
                element.style.flexBasis = flexBasis ?? StyleKeyword.Null;
                
                StyleEnum<T> VToEnum<T>(T v) where T: struct, IConvertible => v;

                element.style.alignItems = alignItems?.Let(VToEnum) ?? StyleKeyword.Null;
                element.style.alignContent = alignContent?.Let(VToEnum) ?? StyleKeyword.Null;
                
                element.style.justifyContent = justifyContent?.Let(VToEnum) ?? StyleKeyword.Null;

                element.style.width = width ?? StyleKeyword.Null;
                element.style.height = height ?? StyleKeyword.Null;
                element.style.minWidth = minWidth ?? StyleKeyword.Null;
                element.style.minHeight = minHeight ?? StyleKeyword.Null;
                element.style.maxWidth = maxWidth ?? StyleKeyword.Null;
                element.style.maxHeight = maxHeight ?? StyleKeyword.Null;
                
                element.style.color = color ?? StyleKeyword.Null;
                element.style.backgroundColor = backgroundColor ?? StyleKeyword.Null;
                element.style.backgroundImage = backgroundImage ?? StyleKeyword.Null;
                
                element.style.paddingLeft = padding.Left ?? StyleKeyword.Null;
                element.style.paddingRight = padding.Right ?? StyleKeyword.Null;
                element.style.paddingTop = padding.Top ?? StyleKeyword.Null;
                element.style.paddingBottom = padding.Bottom ?? StyleKeyword.Null;
                
                element.style.marginLeft = margin.Left ?? StyleKeyword.Null;
                element.style.marginRight = margin.Right ?? StyleKeyword.Null;
                element.style.marginTop = margin.Top ?? StyleKeyword.Null;
                element.style.marginBottom = margin.Bottom ?? StyleKeyword.Null;

                var onClickCallback = onClick;
                var onKeyDownCallback = onKeyDown;
                var onKeyUpCallback = onKeyUp;
                var onBlurCallback = onBlur;
                var onFocusCallback = onFocus;

                var ret = new Events
                {
                    OnClick = onClickCallback != null ? _ => onClickCallback() : null,
                    OnKeyDown = onKeyDownCallback != null
                        ? evt => onKeyDownCallback(new SyntheticKeyEvent(
                            type: SyntheticKeyEvent.EventType.Down,
                            character: evt.character,
                            alt: evt.altKey,
                            ctrl: evt.ctrlKey,
                            shift: evt.shiftKey,
                            cmd: evt.commandKey
                        ))
                        : null,
                    OnKeyUp = onKeyUpCallback != null
                        ? evt => onKeyUpCallback(new SyntheticKeyEvent(
                            type: SyntheticKeyEvent.EventType.Up,
                            character: evt.character,
                            alt: evt.altKey,
                            ctrl: evt.ctrlKey,
                            shift: evt.shiftKey,
                            cmd: evt.ctrlKey
                        ))
                        : null,
                    OnBlur = onBlurCallback != null ? _ => onBlurCallback() : null,
                    OnFocus = onFocusCallback != null ? _ => onFocusCallback() : null
                };

                ret.Register(element);
                
                return ret;
            }
        }
        
        public event Action<VisualElement> OnRender;
        
        /// <summary>
        /// Reference to previously rendered element.
        /// </summary>
        /// <remarks>It is not guaranteed that this <see cref="VisualElement"/> will have expected type(for example <see cref="UnityEngine.UIElements.Label"/>) so it should be runtime checked in every function using this field.</remarks>
        /// <seealso cref="Use{T}"/>
        protected VisualElement PreviouslyRendered { get; private set; }

        /// <summary>
        /// Recomposition strategy used by element.
        /// </summary>
        /// <seealso cref="CompositionContext.RecompositionStrategy"/>
        protected virtual CompositionContext.RecompositionStrategy RecompositionStrategy =>
            CompositionContext.RecompositionStrategy.Override;
        
        private readonly Data data;
        private Data.Events lingeringEvents;

        /// <summary>
        /// Constructs <see cref="Element"/> instance.
        /// </summary>
        /// <param name="data">additional element data <seealso cref="Data"/></param>
        /// <returns></returns>
        [PublicAPI] [NotNull] public static Element V(Data data = new()) => new(data);

        public Element(Data data = new())
        {
            this.data = data;
        }
        
        public VisualElement Render()
        {
            var ret = PrepareElement(GetElement(PreviouslyRendered));
            OnRender?.Invoke(ret);
            PreviouslyRendered = ret;
            return ret;
        }

        public void Recompose(CompositionContext context)
        {
            PreviouslyRendered = context.StartFrame(this, RecompositionStrategy);
            
            OnState(context);
            
            context.EndFrame();
        }

        public virtual void Dispose()
        {
            if (PreviouslyRendered != null)
                lingeringEvents.Unregister(PreviouslyRendered);
            PreviouslyRendered = null;
        }

        /// <summary>
        /// Used to obtain <see cref="VisualElement"/> instance. Override this method if you need instance of <see cref="VisualElement"/> subclass.
        /// </summary>
        /// <seealso cref="Use{T}"/>
        /// <remarks>During render, <see cref="PreviouslyRendered"/> is passed to this function and return value is passed to <see cref="PrepareElement"/> to obtain render result. When overriding you don't need to call base implementation.</remarks>
        /// <param name="source">cached element</param>
        /// <returns></returns>
        [PublicAPI] [NotNull] protected virtual VisualElement GetElement([CanBeNull] VisualElement source) => source ?? new VisualElement();

        /// <summary>
        /// Called every composition. Override it if you need to store anything in the state.
        /// </summary>
        /// <remarks>Works similar to <see cref="IComposition.Recompose"/>, but you don't need to call <see cref="CompositionContext.StartFrame"/> and <see cref="CompositionContext.EndFrame"/>. When overriding you don't need to call base implementation.</remarks>
        /// <param name="context">composition context</param>
        [PublicAPI] protected virtual void OnState(CompositionContext context) { }

        /// <summary>
        /// Postprocess method used to apply styles and events.
        /// </summary>
        /// <remarks>If you need to override some styles override this method and modify styles of returned element from base implementation.</remarks>
        /// <param name="target"><see cref="VisualElement"/> to apply styles and callbacks to</param>
        /// <returns>Returns element passed as <paramref name="target"/>.</returns>
        [PublicAPI] [NotNull] protected virtual VisualElement PrepareElement(VisualElement target)
        {
            if (PreviouslyRendered != null)
                lingeringEvents.Unregister(PreviouslyRendered);
            
            lingeringEvents = data.Apply(target);
            return target;
        }

        /// <summary>
        /// Returns given element if it instance of given type, or new instance otherwise.
        /// </summary>
        /// <param name="source">some element, can be null</param>
        /// <typeparam name="T">expected type</typeparam>
        /// <returns></returns>
        [PublicAPI] [NotNull] protected T Use<T>([CanBeNull] VisualElement source) where T: VisualElement, new()
        {
            if (source is T element)
                return element;

            return new T();
        }
    }
}