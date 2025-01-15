using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Li.USS
{
    [PublicAPI]
    public class USS
    {
        public static USS Style => new();
        
        public string ClassName => className ?? GetClassName();
        private string className;
        public string UssText => ussText ?? GetUssText();
        private string ussText;
        
        private SortedDictionary<string, string> properties = new ();

        public override string ToString() => ClassName;
        
        public static implicit operator string(USS uss) => uss.ToString();
        
        public USS() { }

        public USS(USS other) => properties = new(other.properties);


        private string GetClassName()
        {
            using var sha256 = SHA256.Create();
            
            var builder = new StringBuilder();
            
            foreach (var property in properties)
                builder.Append(property.Key).Append(":").Append(property.Value).Append('\n');
            
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(builder.ToString()));
            
            var hash = BitConverter.ToUInt32(hashBytes, 0);
            
            return $"s{Sanitize(hash.ToString())}";
        }

        private string GetUssText()
        {
            var builder = new StringBuilder();
            
            builder.Append('.').Append(ClassName).Append(" {\n").AppendJoin('\n', properties.Select(entry => $"\t{entry.Key}: {entry.Value};")).Append("\n}");
            
            return builder.ToString();
        }

        public USS AlignContent(StyleEnum<Align> alignment) => WithProperty("align-content", alignment, AlignToString);
        public USS AlignItems(StyleEnum<Align> alignment) => WithProperty("align-items", alignment, AlignToString);
        public USS AlignSelf(StyleEnum<Align> alignment) => WithProperty("align-self", alignment, AlignToString);
        public USS BackgroundColor(StyleColor color) => WithColorProperty("background-color", color);
        //StyleBackground BackgroundImage
        //StyleBackgroundPosition BackgroundPositionX
        //StyleBackgroundPosition BackgroundPositionY
        //StyleBackgroundRepeat BackgroundRepeat
        //StyleBackgroundSize BackgroundSize
        public USS BorderBottomColor(StyleColor color) => WithColorProperty("border-bottom-color", color);
        public USS BorderBottomLeftRadius(StyleLength length) => WithLengthProperty("border-bottom-left-radius", length);
        public USS BorderBottomRightRadius(StyleLength length) => WithLengthProperty("border-bottom-right-radius", length);
        public USS BorderBottomWidth(StyleFloat value) => WithFloatProperty("border-bottom-width", value);
        public USS BorderLeftColor(StyleColor color) => WithColorProperty("border-left-color", color);
        public USS BorderLeftWidth(StyleFloat value) => WithFloatProperty("border-left-width", value);
        public USS BorderRightColor(StyleColor color) => WithColorProperty("border-right-color", color);
        public USS BorderRightWidth(StyleFloat value) => WithFloatProperty("border-right-width", value);
        public USS BorderTopColor(StyleColor color) => WithColorProperty("border-top-color", color);
        public USS BorderTopLeftRadius(StyleLength length) => WithLengthProperty("border-top-left-radius", length);
        public USS BorderTopRightRadius(StyleLength length) => WithLengthProperty("border-top-right-radius", length);
        public USS BorderTopWidth(StyleFloat value) => WithFloatProperty("border-top-width", value);
        public USS Bottom(StyleLength length) => WithLengthProperty("bottom", length);
        public USS Color(StyleColor color) => WithColorProperty("color", color);
        //TODO: handle links
        public USS Cursor(StyleEnum<CursorType> cursor) => WithProperty("cursor", cursor, CursorTypeToString);
        public USS Display(StyleEnum<DisplayStyle> style) => WithProperty("display", style, DisplayToString);
        public USS FlexBasis(StyleLength length) => WithLengthProperty("flex-basis", length);
        public USS FlexDirection(StyleEnum<FlexDirection> direction) => WithProperty("flex-direction", direction, FlexDirectionToString);
        public USS FlexGrow(StyleFloat value) => WithFloatProperty("flex-grow", value);
        public USS FlexShrink(StyleFloat value) => WithFloatProperty("flex-shrink", value);
        public USS Wrap(StyleEnum<Wrap> value) => WithProperty("wrap", value, WrapToString);
        public USS FontSize(StyleLength length) => WithLengthProperty("font-size", length);
        public USS Height(StyleLength length) => WithLengthProperty("height", length);
        public USS Justify(StyleEnum<Justify> justify) => WithProperty("justify", justify, JustifyToString);
        public USS Left(StyleLength length) => WithLengthProperty("left", length);
        public USS LetterSpacing(StyleLength length) => WithLengthProperty("letter-spacing", length);
        public USS MarginBottom(StyleLength length) => WithLengthProperty("margin-bottom", length);
        public USS MarginLeft(StyleLength length) => WithLengthProperty("margin-left", length);
        public USS MarginRight(StyleLength length) => WithLengthProperty("margin-right", length);
        public USS MarginTop(StyleLength length) => WithLengthProperty("margin-top", length);
        public USS MaxHeight(StyleLength length) => WithLengthProperty("max-height", length);
        public USS MaxWidth(StyleLength length) => WithLengthProperty("max-width", length);
        public USS MinHeight(StyleLength length) => WithLengthProperty("min-height", length);
        public USS MinWidth(StyleLength length) => WithLengthProperty("min-width", length);
        public USS Opacity(StyleFloat value) => WithFloatProperty("opacity", value);
        public USS Overflow(StyleEnum<Overflow> overflow) => WithProperty("overflow", overflow, OverflowToString);
        public USS PaddingBottom(StyleLength length) => WithLengthProperty("padding-bottom", length);
        public USS PaddingLeft(StyleLength length) => WithLengthProperty("padding-left", length);
        public USS PaddingRight(StyleLength length) => WithLengthProperty("padding-right", length);
        public USS PaddingTop(StyleLength length) => WithLengthProperty("padding-top", length);
        public USS Position(StyleEnum<Position> position) => WithProperty("position", position, PositionToString);
        public USS Right(StyleLength length) => WithLengthProperty("right", length);
        //StyleRotate Rotate
        //StyleScale Scale
        //StyleEnum<TextOverflow> TextOverflow
        //StyleTextShadow TextShadow
        public USS Top(StyleLength length) => WithLengthProperty("top", length);
        //StyleTransformOrigin TransformOrigin
        //StyleList<TimeValue> TransitionDelay
        //StyleList<TimeValue> TransitionDuration
        //StyleList<StylePropertyName> TransitionProperty
        //StyleList<EasingFunction> TransitionTimingFunction
        //StyleTranslate Translate
        public USS BackgroundImageTintColor(StyleColor color) => WithColorProperty("-unity-background-image-tint-color", color);
        //StyleFont Font
        //StyleFontDefinition UnityFontDefinition
        //StyleEnum<FontStyle> UnityFontStyleAndWeight
        //StyleEnum<OverflowClipBox> UnityOverflowClipBox
        public USS ParagraphSpacing(StyleLength length) => WithLengthProperty("-unity-paragraph-spacing", length);
        public USS SliceBottom(StyleInt value) => WithIntProperty("-unity-slice-bottom", value);
        public USS SliceLeft(StyleInt value) => WithIntProperty("-unity-slice-left", value);
        public USS SliceRight(StyleInt value) => WithIntProperty("-unity-slice-right", value);
        public USS SliceScale(StyleFloat value) => WithFloatProperty("-unity-slice-scale", value);
        public USS SliceTop(StyleInt value) => WithIntProperty("-unity-slice-top", value);
        //StyleEnum<TextAnchor> UnityTextAlign
        public USS TextOutlineColor(StyleColor color) => WithColorProperty("-unity-text-outline-color", color);
        public USS TextOutlineWidth(StyleFloat value) => WithFloatProperty("-unity-text-outline-width", value);
        //StyleEnum<TextOverflowPosition> UnityTextOverflowPosition
        //StyleEnum<Visibility> Visibility
        //StyleEnum<WhiteSpace> WhiteSpace
        public USS Width(StyleLength length) => WithLengthProperty("width", length);
        public USS WordSpacing(StyleLength length) => WithLengthProperty("word-spacing", length);

        private USS WithLengthProperty(string key, StyleLength value) =>
            WithProperty(key, value, LengthToString);
        private USS WithFloatProperty(string key, StyleFloat value) =>
            WithProperty(key, value, FloatToString);
        private USS WithIntProperty(string key, StyleInt value) =>
            WithProperty(key, value, IntToString);
        private USS WithColorProperty(string key, StyleColor value) =>
            WithProperty(key, value, ColorToString);
        
        private USS WithProperty<T>(string key, IStyleValue<T> value, [NotNull] Func<T, string> converter) =>
            WithProperty(key, ValueToString(value, converter));

        private USS WithProperty(string name, string value)
        {
            var newUSS = new USS(this);
            
            if (value != null)
                newUSS.properties[name] = value;
            
            return newUSS;
        }

        private static string ValueToString<T>(IStyleValue<T> value, [NotNull] Func<T, string> converter) =>
            value.keyword switch
            {
                StyleKeyword.Auto => "auto",
                StyleKeyword.Initial => "initial",
                StyleKeyword.None => "none",
                _ => converter(value.value)
            };
        
        private static string IntToString(int value) => value.ToString(CultureInfo.InvariantCulture);
        private static string FloatToString(float value) => value.ToString(CultureInfo.InvariantCulture);
        private static string LengthToString(Length length) => length.ToString();

        private static string PositionToString(Position position) =>
            position switch
            {
                UnityEngine.UIElements.Position.Absolute => "absolute",
                UnityEngine.UIElements.Position.Relative => "relative",
                _ => "relative"
            };
        private static string OverflowToString(Overflow overflow) =>
            overflow switch
            {
                UnityEngine.UIElements.Overflow.Hidden => "hidden",
                UnityEngine.UIElements.Overflow.Visible => "visible",
                _ => "hidden"
            };
        private static string JustifyToString(Justify justify) =>
            justify switch
            {
                UnityEngine.UIElements.Justify.Center => "center",
                UnityEngine.UIElements.Justify.FlexEnd => "flex-end",
                UnityEngine.UIElements.Justify.FlexStart => "flex-start",
                UnityEngine.UIElements.Justify.SpaceAround => "space-around",
                UnityEngine.UIElements.Justify.SpaceBetween => "space-between",
                UnityEngine.UIElements.Justify.SpaceEvenly => "space-evenly",
                _ => "center"
            };
        private static string WrapToString(Wrap wrap) =>
            wrap switch
            {
                UnityEngine.UIElements.Wrap.Wrap => "wrap",
                UnityEngine.UIElements.Wrap.NoWrap => "no-wrap",
                UnityEngine.UIElements.Wrap.WrapReverse => "wrap-reverse",
                _ => "wrap"
            };
        private static string FlexDirectionToString(FlexDirection value) =>
            value switch
            {
                UnityEngine.UIElements.FlexDirection.Column => "column",
                UnityEngine.UIElements.FlexDirection.Row => "row",
                UnityEngine.UIElements.FlexDirection.ColumnReverse => "column-reverse",
                UnityEngine.UIElements.FlexDirection.RowReverse => "row-reverse",
                _ => "row"
            };
        private static string CursorTypeToString(CursorType type) =>
            type switch
            {
                CursorType.Text => "text",
                CursorType.ResizeVertical => "resize-vertical",
                CursorType.ResizeHorizontal => "resize-horizontal",
                CursorType.Link => "link",
                CursorType.SlideArrow => "slide-arrow",
                CursorType.ResizeUpRight => "resize-up-right",
                CursorType.ResizeUpLeft => "resize-up-left",
                CursorType.MoveArrow => "move-arrow",
                CursorType.RotateArrow => "rotate-arrow",
                CursorType.ScaleArrow => "scale-arrow",
                CursorType.ArrowPlus => "arrow-plus",
                CursorType.ArrowMinus => "arrow-minus",
                CursorType.Pan => "pan",
                CursorType.Orbit => "orbit",
                CursorType.Zoom => "zoom",
                CursorType.Fps => "fps",
                CursorType.SplitResizeUpDown => "split-resize-up-down",
                CursorType.SplitResizeLeftRight => "split-resize-left-right",
                CursorType.Arrow => "arrow",
                _ => "arrow",
            };
        private static string DisplayToString(DisplayStyle style) =>
            style switch
            {
                DisplayStyle.None => "none",
                _ => "flex",
            };
        private static string AlignToString(Align alignment) =>
            alignment switch
            {
                Align.Center => "center",
                Align.Stretch => "stretch",
                Align.FlexEnd => "flex-end",
                Align.FlexStart => "flex-start",
                Align.Auto => "auto",
                _ => "auto"
            };
        private static string ColorToString(Color color) =>
            $"rgba({color.r*255:0}, {color.g*255:0}, {color.b*255:0}, {color.a:0.#})";
        
        private static string Sanitize(string value) =>
            value.Replace("-", "_");
    }

    //TODO: change to proper cursor type and handle resource links
    public enum CursorType
    {
        Arrow,
        Text,
        ResizeVertical,
        ResizeHorizontal,
        Link,
        SlideArrow,
        ResizeUpRight,
        ResizeUpLeft,
        MoveArrow,
        RotateArrow,
        ScaleArrow,
        ArrowPlus,
        ArrowMinus,
        Pan,
        Orbit,
        Zoom,
        Fps,
        SplitResizeUpDown,
        SplitResizeLeftRight
    }
}