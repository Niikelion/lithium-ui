using System;
using JetBrains.Annotations;
using UI.Li.Utils;
using UI.Li.Utils.Continuations;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Li.Common
{
    public delegate IComponent StyleFunc(IComponent component);

    [PublicAPI]
    public static class StyleExtensions
    {
        public static IComponent WithStyle(this IComponent obj, Style style) => new StyleWrapper(obj, style);

        public static IComponent WithConditionalStyle(this IComponent obj, bool condition, Style style) =>
            condition ? obj.WithStyle(style) : obj;
        public static IComponent S(this IComponent obj, StyleFunc style) => style(obj);
        public static IComponent Cs(this IComponent obj, bool condition, StyleFunc style) => obj.When(condition, style);
        
        public static Func<IComponent> S<TC>(this StyleFunc style, Func<TC> cmp) where TC: IComponent => () => style(cmp());
        public static Func<T1, IComponent> S<TC, T1>(this StyleFunc style, Func<T1, TC> cmp) where TC: IComponent => arg1 => style(cmp(arg1));
        public static Func<T1, T2, IComponent> S<TC, T1, T2>(this StyleFunc style, Func<T1, T2, TC> cmp) where TC: IComponent => (arg1, arg2) => style(cmp(arg1, arg2));
        public static Func<T1, T2, T3, IComponent> S<TC, T1, T2, T3>(this StyleFunc style, Func<T1, T2, T3, TC> cmp) where TC: IComponent => (arg1, arg2, arg3) => style(cmp(arg1, arg2, arg3));
        public static Func<T1, T2, T3, T4, IComponent> S<TC, T1, T2, T3, T4>(this StyleFunc style, Func<T1, T2, T3, T4, TC> cmp) where TC: IComponent => (arg1, arg2, arg3, arg4) => style(cmp(arg1, arg2, arg3, arg4));
        public static Func<T1, T2, T3, T4, T5, IComponent> S<TC, T1, T2, T3, T4, T5>(this StyleFunc style, Func<T1, T2, T3, T4, T5, TC> cmp) where TC: IComponent => (arg1, arg2, arg3, arg4, arg5) => style(cmp(arg1, arg2, arg3, arg4, arg5));
        public static Func<T1, T2, T3, T4, T5, T6, IComponent> S<TC, T1, T2, T3, T4, T5, T6>(this StyleFunc style, Func<T1, T2, T3, T4, T5, T6, TC> cmp) where TC: IComponent => (arg1, arg2, arg3, arg4, arg5, arg6) => style(cmp(arg1, arg2, arg3, arg4, arg5, arg6));
        public static Func<T1, T2, T3, T4, T5, T6, T7, IComponent> S<TC, T1, T2, T3, T4, T5, T6, T7>(this StyleFunc style, Func<T1, T2, T3, T4, T5, T6, T7, TC> cmp) where TC: IComponent => (arg1, arg2, arg3, arg4, arg5, arg6, arg7) => style(cmp(arg1, arg2, arg3, arg4, arg5, arg6, arg7));
        public static Func<T1, T2, T3, T4, T5, T6, T7, T8, IComponent> S<TC, T1, T2, T3, T4, T5, T6, T7, T8>(this StyleFunc style, Func<T1, T2, T3, T4, T5, T6, T7, T8, TC> cmp) where TC: IComponent => (arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8) => style(cmp(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8));
    }
    
    [PublicAPI]
    public struct Style
    {
        [PublicAPI]
        public struct Frame
        {
            public readonly StyleLength? Left, Right, Top, Bottom;

            public static implicit operator Frame(int s) => new (s);
            public static implicit operator Frame((int Horizontal, int Vertical) s) => new (s.Horizontal, s.Vertical);
            public static implicit operator Frame((int Left, int Right, int Top, int Bottom) s) => new (s.Left, s.Right, s.Top, s.Bottom);
            
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

            public Frame(StyleLength? size) : this(horizontal: size, vertical: size) { }
        }
        
        public StyleEnum<Align>? AlignContent { get; set; }
        public StyleEnum<Align>? AlignItems { get; set; }
        public StyleEnum<Align>? AlignSelf { get; set; }
        public StyleColor? BackgroundColor { get; set; }
        public StyleBackground? BackgroundImage { get; set; }
        public StyleBackgroundPosition? BackgroundPositionX { get; set; }
        public StyleBackgroundPosition? BackgroundPositionY { get; set; }
        public StyleBackgroundRepeat? BackgroundRepeat { get; set; }
        public StyleBackgroundSize? BackgroundSize { get; set; }
        public StyleColor? BorderBottomColor { get; set; }
        public StyleLength? BorderBottomLeftRadius  { get; set; }
        public StyleLength? BorderBottomRightRadius  { get; set; }
        public StyleFloat? BorderBottomWidth  { get; set; }
        public StyleColor? BorderLeftColor { get; set; }
        public StyleFloat? BorderLeftWidth  { get; set; }
        public StyleColor? BorderRightColor { get; set; }
        public StyleFloat? BorderRightWidth  { get; set; }
        public StyleColor? BorderTopColor { get; set; }
        public StyleLength? BorderTopLeftRadius  { get; set; }
        public StyleLength? BorderTopRightRadius  { get; set; }
        public StyleFloat? BorderTopWidth  { get; set; }
        public StyleLength? Bottom  { get; set; }
        public StyleColor? Color { get; set; }
        public StyleCursor? Cursor { get; set; }
        public StyleEnum<DisplayStyle>? Display { get; set; }
        public StyleLength? FlexBasis  { get; set; }
        public StyleEnum<FlexDirection>? FlexDirection { get; set; }
        public StyleFloat? FlexGrow  { get; set; }
        public StyleFloat? FlexShrink  { get; set; }
        public StyleEnum<Wrap>? FlexWrap { get; set; }
        public StyleLength? FontSize  { get; set; }
        public StyleLength? Height  { get; set; }
        public StyleEnum<Justify>? JustifyContent { get; set; }
        public StyleLength? Left  { get; set; }
        public StyleLength? LetterSpacing  { get; set; }
        public Frame? Margin { get; set; }
        public StyleLength? MaxHeight  { get; set; }
        public StyleLength? MaxWidth  { get; set; }
        public StyleLength? MinHeight  { get; set; }
        public StyleLength? MinWidth  { get; set; }
        public StyleFloat? Opacity  { get; set; }
        public StyleEnum<Overflow>? Overflow { get; set; }
        public Frame? Padding { get; set; }
        public StyleEnum<Position>? Position { get; set; }
        public StyleLength? Right  { get; set; }
        public StyleRotate? Rotate { get; set; }
        public StyleScale? Scale { get; set; }
        public StyleEnum<TextOverflow>? TextOverflow { get; set; }
        public StyleTextShadow? TextShadow { get; set; }
        public StyleLength? Top  { get; set; }
        public StyleTransformOrigin? TransformOrigin { get; set; }
        public StyleList<TimeValue>? TransitionDelay { get; set; }
        public StyleList<TimeValue>? TransitionDuration { get; set; }
        public StyleList<StylePropertyName>? TransitionProperty { get; set; }
        public StyleList<EasingFunction>? TransitionTimingFunction { get; set; }
        public StyleTranslate? Translate { get; set; }
        public StyleColor? UnityBackgroundImageTintColor { get; set; }
        public StyleFont? UnityFont { get; set; }
        public StyleFontDefinition? UnityFontDefinition { get; set; }
        public StyleEnum<FontStyle>? UnityFontStyleAndWeight { get; set; }
        public StyleEnum<OverflowClipBox>? UnityOverflowClipBox { get; set; }
        public StyleLength? UnityParagraphSpacing  { get; set; }
        public StyleInt? UnitySliceBottom  { get; set; }
        public StyleInt? UnitySliceLeft  { get; set; }
        public StyleInt? UnitySliceRight  { get; set; }
        public StyleFloat? UnitySliceScale  { get; set; }
        public StyleInt? UnitySliceTop  { get; set; }
        public StyleEnum<TextAnchor>? UnityTextAlign { get; set; }
        public StyleColor? UnityTextOutlineColor { get; set; }
        public StyleFloat? UnityTextOutlineWidth  { get; set; }
        public StyleEnum<TextOverflowPosition>? UnityTextOverflowPosition { get; set; }
        public StyleEnum<Visibility>? Visibility { get; set; }
        public StyleEnum<WhiteSpace>? WhiteSpace { get; set; }
        public StyleLength? Width  { get; set; }
        public StyleLength? WordSpacing  { get; set; }
        
        public static Style CopyFromElement(VisualElement element)
        {
            var ret = new Style(null)
            {
                AlignContent = element.style.alignContent,
                AlignItems = element.style.alignItems,
                AlignSelf = element.style.alignSelf,
                BackgroundColor = element.style.backgroundColor,
                BackgroundImage = element.style.backgroundImage,
                BackgroundPositionX = element.style.backgroundPositionX,
                BackgroundPositionY = element.style.backgroundPositionY,
                BackgroundRepeat = element.style.backgroundRepeat,
                BackgroundSize = element.style.backgroundSize,
                BorderBottomColor = element.style.borderBottomColor,
                BorderBottomLeftRadius = element.style.borderBottomLeftRadius,
                BorderBottomRightRadius = element.style.borderBottomRightRadius,
                BorderBottomWidth = element.style.borderBottomWidth,
                BorderLeftColor = element.style.borderLeftColor,
                BorderLeftWidth = element.style.borderLeftWidth,
                BorderRightColor = element.style.borderRightColor,
                BorderRightWidth = element.style.borderRightWidth,
                BorderTopColor = element.style.borderTopColor,
                BorderTopLeftRadius = element.style.borderTopLeftRadius,
                BorderTopRightRadius = element.style.borderTopRightRadius,
                BorderTopWidth = element.style.borderTopWidth,
                Bottom = element.style.bottom,
                Color = element.style.color,
                Cursor = element.style.cursor,
                Display = element.style.display,
                FlexBasis = element.style.flexBasis,
                FlexDirection = element.style.flexDirection,
                FlexGrow = element.style.flexGrow,
                FlexShrink = element.style.flexShrink,
                FlexWrap = element.style.flexWrap,
                FontSize = element.style.fontSize,
                Height = element.style.height,
                JustifyContent = element.style.justifyContent,
                Left = element.style.left,
                LetterSpacing = element.style.letterSpacing,
                MaxHeight = element.style.maxHeight,
                MaxWidth = element.style.maxWidth,
                MinHeight = element.style.minHeight,
                MinWidth = element.style.minWidth,
                Opacity = element.style.opacity,
                Overflow = element.style.overflow,
                Position = element.style.position,
                Right = element.style.right,
                Rotate = element.style.rotate,
                Scale = element.style.scale,
                TextOverflow = element.style.textOverflow,
                TextShadow = element.style.textShadow,
                Top = element.style.top,
                TransformOrigin = element.style.transformOrigin,
                TransitionDelay = element.style.transitionDelay,
                TransitionDuration = element.style.transitionDuration,
                TransitionProperty = element.style.transitionProperty,
                TransitionTimingFunction = element.style.transitionTimingFunction,
                Translate = element.style.translate,
                UnityBackgroundImageTintColor = element.style.unityBackgroundImageTintColor,
                UnityFont = element.style.unityFont,
                UnityFontDefinition = element.style.unityFontDefinition,
                UnityFontStyleAndWeight = element.style.unityFontStyleAndWeight,
                UnityOverflowClipBox = element.style.unityOverflowClipBox,
                UnityParagraphSpacing = element.style.unityParagraphSpacing,
                UnitySliceBottom = element.style.unitySliceBottom,
                UnitySliceLeft = element.style.unitySliceLeft,
                UnitySliceRight = element.style.unitySliceRight,
                UnitySliceScale = element.style.unitySliceScale,
                UnitySliceTop = element.style.unitySliceTop,
                UnityTextAlign = element.style.unityTextAlign,
                UnityTextOutlineColor = element.style.unityTextOutlineColor,
                UnityTextOutlineWidth = element.style.unityTextOutlineWidth,
                UnityTextOverflowPosition = element.style.unityTextOverflowPosition,
                Visibility = element.style.visibility,
                WhiteSpace = element.style.whiteSpace,
                Width = element.style.width,
                WordSpacing = element.style.wordSpacing,
                Margin = new (element.style.marginLeft, element.style.marginRight, element.style.marginTop, element.style.marginBottom),
                Padding = new (element.style.paddingLeft, element.style.paddingRight, element.style.paddingTop, element.style.paddingBottom)
            };

            return ret;
        }
        
        // ReSharper disable once PureAttributeOnVoidMethod
        [Pure] public void ApplyToElement(VisualElement element)
        {
            if (AlignContent.HasValue) element.style.alignContent = AlignContent.Value;
            if (AlignItems.HasValue) element.style.alignItems = AlignItems.Value;
            if (AlignSelf.HasValue) element.style.alignSelf = AlignSelf.Value;
            if (BackgroundColor.HasValue) element.style.backgroundColor = BackgroundColor.Value;
            if (BackgroundImage.HasValue) element.style.backgroundImage = BackgroundImage.Value;
            if (BackgroundPositionX.HasValue) element.style.backgroundPositionX = BackgroundPositionX.Value;
            if (BackgroundPositionY.HasValue) element.style.backgroundPositionY = BackgroundPositionY.Value;
            if (BackgroundRepeat.HasValue) element.style.backgroundRepeat = BackgroundRepeat.Value;
            if (BackgroundSize.HasValue) element.style.backgroundSize = BackgroundSize.Value;
            if (BorderBottomColor.HasValue) element.style.borderBottomColor = BorderBottomColor.Value;
            if (BorderBottomLeftRadius.HasValue) element.style.borderBottomLeftRadius = BorderBottomLeftRadius.Value;
            if (BorderBottomRightRadius.HasValue) element.style.borderBottomRightRadius = BorderBottomRightRadius.Value;
            if (BorderBottomWidth.HasValue) element.style.borderBottomWidth = BorderBottomWidth.Value;
            if (BorderLeftColor.HasValue) element.style.borderLeftColor = BorderLeftColor.Value;
            if (BorderLeftWidth.HasValue) element.style.borderLeftWidth = BorderLeftWidth.Value;
            if (BorderRightColor.HasValue) element.style.borderRightColor = BorderRightColor.Value;
            if (BorderRightWidth.HasValue) element.style.borderRightWidth = BorderRightWidth.Value;
            if (BorderTopColor.HasValue) element.style.borderTopColor = BorderTopColor.Value;
            if (BorderTopLeftRadius.HasValue) element.style.borderTopLeftRadius = BorderTopLeftRadius.Value;
            if (BorderTopRightRadius.HasValue) element.style.borderTopRightRadius = BorderTopRightRadius.Value;
            if (BorderTopWidth.HasValue) element.style.borderTopWidth = BorderTopWidth.Value;
            if (Bottom.HasValue) element.style.bottom = Bottom.Value;
            if (Color.HasValue) element.style.color = Color.Value;
            if (Cursor.HasValue) element.style.cursor = Cursor.Value;
            if (Display.HasValue) element.style.display = Display.Value;
            if (FlexBasis.HasValue) element.style.flexBasis = FlexBasis.Value;
            if (FlexDirection.HasValue) element.style.flexDirection = FlexDirection.Value;
            if (FlexGrow.HasValue) element.style.flexGrow = FlexGrow.Value;
            if (FlexShrink.HasValue) element.style.flexShrink = FlexShrink.Value;
            if (FlexWrap.HasValue) element.style.flexWrap = FlexWrap.Value;
            if (FontSize.HasValue) element.style.fontSize = FontSize.Value;
            if (Height.HasValue) element.style.height = Height.Value;
            if (JustifyContent.HasValue) element.style.justifyContent = JustifyContent.Value;
            if (Left.HasValue) element.style.left = Left.Value;
            if (LetterSpacing.HasValue) element.style.letterSpacing = LetterSpacing.Value;
            if (MaxHeight.HasValue) element.style.maxHeight = MaxHeight.Value;
            if (MaxWidth.HasValue) element.style.maxWidth = MaxWidth.Value;
            if (MinHeight.HasValue) element.style.minHeight = MinHeight.Value;
            if (MinWidth.HasValue) element.style.minWidth = MinWidth.Value;
            if (Opacity.HasValue) element.style.opacity = Opacity.Value;
            if (Overflow.HasValue) element.style.overflow = Overflow.Value;
            if (Position.HasValue) element.style.position = Position.Value;
            if (Right.HasValue) element.style.right = Right.Value;
            if (Rotate.HasValue) element.style.rotate = Rotate.Value;
            if (Scale.HasValue) element.style.scale = Scale.Value;
            if (TextOverflow.HasValue) element.style.textOverflow = TextOverflow.Value;
            if (TextShadow.HasValue) element.style.textShadow = TextShadow.Value;
            if (Top.HasValue) element.style.top = Top.Value;
            if (TransformOrigin.HasValue) element.style.transformOrigin = TransformOrigin.Value;
            if (TransitionDelay.HasValue) element.style.transitionDelay = TransitionDelay.Value;
            if (TransitionDuration.HasValue) element.style.transitionDuration = TransitionDuration.Value;
            if (TransitionProperty.HasValue) element.style.transitionProperty = TransitionProperty.Value;
            if (TransitionTimingFunction.HasValue) element.style.transitionTimingFunction = TransitionTimingFunction.Value;
            if (Translate.HasValue) element.style.translate = Translate.Value;
            if (UnityBackgroundImageTintColor.HasValue) element.style.unityBackgroundImageTintColor = UnityBackgroundImageTintColor.Value;
            if (UnityFont.HasValue) element.style.unityFont = UnityFont.Value;
            if (UnityFontDefinition.HasValue) element.style.unityFontDefinition = UnityFontDefinition.Value;
            if (UnityFontStyleAndWeight.HasValue) element.style.unityFontStyleAndWeight = UnityFontStyleAndWeight.Value;
            if (UnityOverflowClipBox.HasValue) element.style.unityOverflowClipBox = UnityOverflowClipBox.Value;
            if (UnityParagraphSpacing.HasValue) element.style.unityParagraphSpacing = UnityParagraphSpacing.Value;
            if (UnitySliceBottom.HasValue) element.style.unitySliceBottom = UnitySliceBottom.Value;
            if (UnitySliceLeft.HasValue) element.style.unitySliceLeft = UnitySliceLeft.Value;
            if (UnitySliceRight.HasValue) element.style.unitySliceRight = UnitySliceRight.Value;
            if (UnitySliceScale.HasValue) element.style.unitySliceScale = UnitySliceScale.Value;
            if (UnitySliceTop.HasValue) element.style.unitySliceTop = UnitySliceTop.Value;
            if (UnityTextAlign.HasValue) element.style.unityTextAlign = UnityTextAlign.Value;
            if (UnityTextOutlineColor.HasValue) element.style.unityTextOutlineColor = UnityTextOutlineColor.Value;
            if (UnityTextOutlineWidth.HasValue) element.style.unityTextOutlineWidth = UnityTextOutlineWidth.Value;
            if (UnityTextOverflowPosition.HasValue) element.style.unityTextOverflowPosition = UnityTextOverflowPosition.Value;
            if (Visibility.HasValue) element.style.visibility = Visibility.Value;
            if (WhiteSpace.HasValue) element.style.whiteSpace = WhiteSpace.Value;
            if (Width.HasValue) element.style.width = Width.Value;
            if (WordSpacing.HasValue) element.style.wordSpacing = WordSpacing.Value;
            
            if (Margin?.Left != null) element.style.marginLeft = Margin.Value.Left.Value;
            if (Margin?.Right != null) element.style.marginRight = Margin.Value.Right.Value;
            if (Margin?.Top != null) element.style.marginTop = Margin.Value.Top.Value;
            if (Margin?.Bottom != null) element.style.marginBottom = Margin.Value.Bottom.Value;

            if (Padding?.Left != null) element.style.paddingLeft = Padding.Value.Left.Value;
            if (Padding?.Right != null) element.style.paddingRight = Padding.Value.Right.Value;
            if (Padding?.Top != null) element.style.paddingTop = Padding.Value.Top.Value;
            if (Padding?.Bottom != null) element.style.paddingBottom = Padding.Value.Bottom.Value;

            element.MarkDirtyRepaint();
        }

        public Style(
            Style? parentStyle = null,
            StyleEnum<Align>? alignContent = null,
            StyleEnum<Align>? alignItems = null,
            StyleEnum<Align>? alignSelf = null,
            StyleColor? backgroundColor = null,
            StyleBackground? backgroundImage = null,
            StyleBackgroundPosition? backgroundPositionX = null,
            StyleBackgroundPosition? backgroundPositionY = null,
            StyleBackgroundRepeat? backgroundRepeat = null,
            StyleBackgroundSize? backgroundSize = null,
            StyleColor? borderBottomColor = null,
            StyleLength? borderBottomLeftRadius = null,
            StyleLength? borderBottomRightRadius = null,
            StyleFloat? borderBottomWidth = null,
            StyleColor? borderLeftColor = null,
            StyleFloat? borderLeftWidth = null,
            StyleColor? borderRightColor = null,
            StyleFloat? borderRightWidth = null,
            StyleColor? borderTopColor = null,
            StyleLength? borderTopLeftRadius = null,
            StyleLength? borderTopRightRadius = null,
            StyleFloat? borderTopWidth = null,
            StyleLength? bottom = null,
            StyleColor? color = null,
            StyleCursor? cursor = null,
            StyleEnum<DisplayStyle>? display = null,
            StyleLength? flexBasis = null,
            StyleEnum<FlexDirection>? flexDirection = null,
            StyleFloat? flexGrow = null,
            StyleFloat? flexShrink = null,
            StyleEnum<Wrap>? flexWrap = null,
            StyleLength? fontSize = null,
            StyleLength? height = null,
            StyleEnum<Justify>? justifyContent = null,
            StyleLength? left = null,
            StyleLength? letterSpacing = null,
            StyleLength? maxHeight = null,
            StyleLength? maxWidth = null,
            StyleLength? minHeight = null,
            StyleLength? minWidth = null,
            StyleFloat? opacity = null,
            StyleEnum<Overflow>? overflow = null,
            StyleEnum<Position>? position = null,
            StyleLength? right = null,
            StyleRotate? rotate = null,
            StyleScale? scale = null,
            StyleEnum<TextOverflow>? textOverflow = null,
            StyleTextShadow? textShadow = null,
            StyleLength? top = null,
            StyleTransformOrigin? transformOrigin = null,
            StyleList<TimeValue>? transitionDelay = null,
            StyleList<TimeValue>? transitionDuration = null,
            StyleList<StylePropertyName>? transitionProperty = null,
            StyleList<EasingFunction>? transitionTimingFunction = null,
            StyleTranslate? translate = null,
            StyleColor? unityBackgroundImageTintColor = null,
            StyleFont? unityFont = null,
            StyleFontDefinition? unityFontDefinition = null,
            StyleEnum<FontStyle>? unityFontStyleAndWeight = null,
            StyleEnum<OverflowClipBox>? unityOverflowClipBox = null,
            StyleLength? unityParagraphSpacing = null,
            StyleInt? unitySliceBottom = null,
            StyleInt? unitySliceLeft = null,
            StyleInt? unitySliceRight = null,
            StyleFloat? unitySliceScale = null,
            StyleInt? unitySliceTop = null,
            StyleEnum<TextAnchor>? unityTextAlign = null,
            StyleColor? unityTextOutlineColor = null,
            StyleFloat? unityTextOutlineWidth = null,
            StyleEnum<TextOverflowPosition>? unityTextOverflowPosition = null,
            StyleEnum<Visibility>? visibility = null,
            StyleEnum<WhiteSpace>? whiteSpace = null,
            StyleLength? width = null,
            StyleLength? wordSpacing = null,
            Frame? margin = null,
            Frame? padding = null
        )
        {
            AlignContent = alignContent ?? parentStyle?.AlignContent;
            AlignItems = alignItems ?? parentStyle?.AlignItems;
            AlignSelf = alignSelf ?? parentStyle?.AlignSelf;
            BackgroundColor = backgroundColor ?? parentStyle?.BackgroundColor;
            BackgroundImage = backgroundImage ?? parentStyle?.BackgroundImage;
            BackgroundPositionX = backgroundPositionX ?? parentStyle?.BackgroundPositionX;
            BackgroundPositionY = backgroundPositionY ?? parentStyle?.BackgroundPositionY;
            BackgroundRepeat = backgroundRepeat ?? parentStyle?.BackgroundRepeat;
            BackgroundSize = backgroundSize ?? parentStyle?.BackgroundSize;
            BorderBottomColor = borderBottomColor ?? parentStyle?.BorderBottomColor;
            BorderBottomLeftRadius = borderBottomLeftRadius ?? parentStyle?.BorderBottomLeftRadius;
            BorderBottomRightRadius = borderBottomRightRadius ?? parentStyle?.BorderBottomRightRadius;
            BorderBottomWidth = borderBottomWidth ?? parentStyle?.BorderBottomWidth;
            BorderLeftColor = borderLeftColor ?? parentStyle?.BorderLeftColor;
            BorderLeftWidth = borderLeftWidth ?? parentStyle?.BorderLeftWidth;
            BorderRightColor = borderRightColor ?? parentStyle?.BorderRightColor;
            BorderRightWidth = borderRightWidth ?? parentStyle?.BorderRightWidth;
            BorderTopColor = borderTopColor ?? parentStyle?.BorderTopColor;
            BorderTopLeftRadius = borderTopLeftRadius ?? parentStyle?.BorderTopLeftRadius;
            BorderTopRightRadius = borderTopRightRadius ?? parentStyle?.BorderTopRightRadius;
            BorderTopWidth = borderTopWidth ?? parentStyle?.BorderTopWidth;
            Bottom = bottom ?? parentStyle?.Bottom;
            Color = color ?? parentStyle?.Color;
            Cursor = cursor ?? parentStyle?.Cursor;
            Display = display ?? parentStyle?.Display;
            FlexBasis = flexBasis ?? parentStyle?.FlexBasis;
            FlexDirection = flexDirection ?? parentStyle?.FlexDirection;
            FlexGrow = flexGrow ?? parentStyle?.FlexGrow;
            FlexShrink = flexShrink ?? parentStyle?.FlexShrink;
            FlexWrap = flexWrap ?? parentStyle?.FlexWrap;
            FontSize = fontSize ?? parentStyle?.FontSize;
            Height = height ?? parentStyle?.Height;
            JustifyContent = justifyContent ?? parentStyle?.JustifyContent;
            Left = left ?? parentStyle?.Left;
            LetterSpacing = letterSpacing ?? parentStyle?.LetterSpacing;
            MaxHeight = maxHeight ?? parentStyle?.MaxHeight;
            MaxWidth = maxWidth ?? parentStyle?.MaxWidth;
            MinHeight = minHeight ?? parentStyle?.MinHeight;
            MinWidth = minWidth ?? parentStyle?.MinWidth;
            Opacity = opacity ?? parentStyle?.Opacity;
            Overflow = overflow ?? parentStyle?.Overflow;
            Position = position ?? parentStyle?.Position;
            Right = right ?? parentStyle?.Right;
            Rotate = rotate ?? parentStyle?.Rotate;
            Scale = scale ?? parentStyle?.Scale;
            TextOverflow = textOverflow ?? parentStyle?.TextOverflow;
            TextShadow = textShadow ?? parentStyle?.TextShadow;
            Top = top ?? parentStyle?.Top;
            TransformOrigin = transformOrigin ?? parentStyle?.TransformOrigin;
            TransitionDelay = transitionDelay ?? parentStyle?.TransitionDelay;
            TransitionDuration = transitionDuration ?? parentStyle?.TransitionDuration;
            TransitionProperty = transitionProperty ?? parentStyle?.TransitionProperty;
            TransitionTimingFunction = transitionTimingFunction ?? parentStyle?.TransitionTimingFunction;
            Translate = translate ?? parentStyle?.Translate;
            UnityBackgroundImageTintColor = unityBackgroundImageTintColor ?? parentStyle?.UnityBackgroundImageTintColor;
            UnityFont = unityFont ?? parentStyle?.UnityFont;
            UnityFontDefinition = unityFontDefinition ?? parentStyle?.UnityFontDefinition;
            UnityFontStyleAndWeight = unityFontStyleAndWeight ?? parentStyle?.UnityFontStyleAndWeight;
            UnityOverflowClipBox = unityOverflowClipBox ?? parentStyle?.UnityOverflowClipBox;
            UnityParagraphSpacing = unityParagraphSpacing ?? parentStyle?.UnityParagraphSpacing;
            UnitySliceBottom = unitySliceBottom ?? parentStyle?.UnitySliceBottom;
            UnitySliceLeft = unitySliceLeft ?? parentStyle?.UnitySliceLeft;
            UnitySliceRight = unitySliceRight ?? parentStyle?.UnitySliceRight;
            UnitySliceScale = unitySliceScale ?? parentStyle?.UnitySliceScale;
            UnitySliceTop = unitySliceTop ?? parentStyle?.UnitySliceTop;
            UnityTextAlign = unityTextAlign ?? parentStyle?.UnityTextAlign;
            UnityTextOutlineColor = unityTextOutlineColor ?? parentStyle?.UnityTextOutlineColor;
            UnityTextOutlineWidth = unityTextOutlineWidth ?? parentStyle?.UnityTextOutlineWidth;
            UnityTextOverflowPosition = unityTextOverflowPosition ?? parentStyle?.UnityTextOverflowPosition;
            Visibility = visibility ?? parentStyle?.Visibility;
            WhiteSpace = whiteSpace ?? parentStyle?.WhiteSpace;
            Width = width ?? parentStyle?.Width;
            WordSpacing = wordSpacing ?? parentStyle?.WordSpacing;
            Margin = margin ?? parentStyle?.Margin;
            Padding = padding ?? parentStyle?.Padding;
        }
    }
}