using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI.Li.USS
{
    [PublicAPI]
    public class USS
    {
        [PublicAPI]
        public class USSBuilder
        {
            private string className;
            private SortedDictionary<string, string> properties = new();
            private SortedDictionary<string, USS> nested = new();

            public USSBuilder(string className = null) => this.className = className;
            
            public static implicit operator USS(USSBuilder builder) => new(builder.className, builder.properties, builder.nested);

            public USSBuilder Select(string selector, USS style)
            {
                nested[selector] = style;
                return this;
            }
            
            public USSBuilder AlignContent(StyleEnum<Align> alignment) => WithProperty("align-content", alignment.AsUssString());
            public USSBuilder AlignItems(StyleEnum<Align> alignment) => WithProperty("align-items", alignment.AsUssString());
            public USSBuilder AlignSelf(StyleEnum<Align> alignment) => WithProperty("align-self", alignment.AsUssString());
            public USSBuilder BackgroundColor(StyleColor color) => WithProperty("background-color", color.AsUssString());
            public USSBuilder BackgroundImage(string path) => WithProperty("background-image", $"url(\"{path.Replace("\"", "\\\"")}\")");
            public USSBuilder BackgroundPositionX(StyleBackgroundPosition x) => WithProperty("background-position-x", x.AsUssString());
            public USSBuilder BackgroundPositionY(StyleBackgroundPosition y) => WithProperty("background-position-y", y.AsUssString());
            public USSBuilder BackgroundRepeat(StyleBackgroundRepeat repeat) => WithProperty("background-repeat", repeat.AsUssString());
            public USSBuilder BackgroundSize(StyleBackgroundSize size) => WithProperty("background-size", size.ToString());
            public USSBuilder BorderBottomColor(StyleColor color) => WithProperty("border-bottom-color", color.AsUssString());
            public USSBuilder BorderBottomLeftRadius(StyleLength length) => WithProperty("border-bottom-left-radius", length.AsUssString());
            public USSBuilder BorderBottomRightRadius(StyleLength length) => WithProperty("border-bottom-right-radius", length.AsUssString());
            public USSBuilder BorderBottomWidth(StyleFloat value) => WithProperty("border-bottom-width", value.AsUssString());
            public USSBuilder BorderLeftColor(StyleColor color) => WithProperty("border-left-color", color.AsUssString());
            public USSBuilder BorderLeftWidth(StyleFloat value) => WithProperty("border-left-width", value.AsUssString());
            public USSBuilder BorderRightColor(StyleColor color) => WithProperty("border-right-color", color.AsUssString());
            public USSBuilder BorderRightWidth(StyleFloat value) => WithProperty("border-right-width", value.AsUssString());
            public USSBuilder BorderTopColor(StyleColor color) => WithProperty("border-top-color", color.AsUssString());
            public USSBuilder BorderTopLeftRadius(StyleLength length) => WithProperty("border-top-left-radius", length.AsUssString());
            public USSBuilder BorderTopRightRadius(StyleLength length) => WithProperty("border-top-right-radius", length.AsUssString());
            public USSBuilder BorderTopWidth(StyleFloat value) => WithProperty("border-top-width", value.AsUssString());
            public USSBuilder Bottom(StyleLength length) => WithProperty("bottom", length.AsUssString());
            public USSBuilder Color(StyleColor color) => WithProperty("color", color.AsUssString());
            //TODO: handle links
            public USSBuilder Cursor(StyleEnum<CursorType> cursor) => WithProperty("cursor", cursor.AsUssString());
            public USSBuilder Display(StyleEnum<DisplayStyle> style) => WithProperty("display", style.AsUssString());
            public USSBuilder FlexBasis(StyleLength length) => WithProperty("flex-basis", length.AsUssString());
            public USSBuilder FlexDirection(StyleEnum<FlexDirection> direction) => WithProperty("flex-direction", direction.AsUssString());

            public USSBuilder FlexGrow(StyleFloat value) => WithProperty("flex-grow", value.AsUssString());
            public USSBuilder FlexShrink(StyleFloat value) => WithProperty("flex-shrink", value.AsUssString());
            public USSBuilder Wrap(StyleEnum<Wrap> value) => WithProperty("wrap", value.AsUssString());
            public USSBuilder FontSize(StyleLength length) => WithProperty("font-size", length.AsUssString());
            public USSBuilder Height(StyleLength length) => WithProperty("height", length.AsUssString());
            public USSBuilder Justify(StyleEnum<Justify> justify) => WithProperty("justify", justify.AsUssString());
            public USSBuilder Left(StyleLength length) => WithProperty("left", length.AsUssString());
            public USSBuilder LetterSpacing(StyleLength length) => WithProperty("letter-spacing", length.AsUssString());
            public USSBuilder MarginBottom(StyleLength length) => WithProperty("margin-bottom", length.AsUssString());
            public USSBuilder MarginLeft(StyleLength length) => WithProperty("margin-left", length.AsUssString());
            public USSBuilder MarginRight(StyleLength length) => WithProperty("margin-right", length.AsUssString());
            public USSBuilder MarginTop(StyleLength length) => WithProperty("margin-top", length.AsUssString());
            public USSBuilder MaxHeight(StyleLength length) => WithProperty("max-height", length.AsUssString());
            public USSBuilder MaxWidth(StyleLength length) => WithProperty("max-width", length.AsUssString());
            public USSBuilder MinHeight(StyleLength length) => WithProperty("min-height", length.AsUssString());
            public USSBuilder MinWidth(StyleLength length) => WithProperty("min-width", length.AsUssString());
            public USSBuilder Opacity(StyleFloat value) => WithProperty("opacity", value.AsUssString());
            public USSBuilder Overflow(StyleEnum<Overflow> overflow) => WithProperty("overflow", overflow.AsUssString());
            public USSBuilder PaddingBottom(StyleLength length) => WithProperty("padding-bottom", length.AsUssString());
            public USSBuilder PaddingLeft(StyleLength length) => WithProperty("padding-left", length.AsUssString());
            public USSBuilder PaddingRight(StyleLength length) => WithProperty("padding-right", length.AsUssString());
            public USSBuilder PaddingTop(StyleLength length) => WithProperty("padding-top", length.AsUssString());
            public USSBuilder Position(StyleEnum<Position> position) => WithProperty("position", position.AsUssString());
            public USSBuilder Right(StyleLength length) => WithProperty("right", length.AsUssString());
            public USSBuilder Rotate(StyleAngle angle) => WithProperty("rotate", angle.AsUssString());
            public USSBuilder Scale(StyleScale scale) => WithProperty("scale", scale.AsUssString());
            public USSBuilder TextOverflow(StyleEnum<TextOverflow> overflow) => WithProperty("text-overflow", overflow.AsUssString());
            public USSBuilder TextShadow(Length xOffset, Length yOffset, Length blurRadius, Color color) =>
                WithProperty("text-shadow", $"{USSExtensions.ToUssString(xOffset)} {USSExtensions.ToUssString(yOffset)} {USSExtensions.ToUssString(blurRadius)} {USSExtensions.ToUssString(color)}");
            public USSBuilder Top(StyleLength length) => WithProperty("top", length.AsUssString());
            public USSBuilder TransformOrigin(Length x, Length y) => WithProperty("transform-origin", $"{USSExtensions.ToUssString(x)} {USSExtensions.ToUssString(y)}");
            public USSBuilder Transition(Transition transition, params Transition[] transitions) =>
                WithProperty("transition", transition + string.Join("", transitions.Select(t => $", {t}")));
            public USSBuilder Translate(Length x, Length y) => WithProperty("translate", $"{USSExtensions.ToUssString(x)} {USSExtensions.ToUssString(y)}");
            public USSBuilder BackgroundImageTintColor(StyleColor color) => WithProperty("-unity-background-image-tint-color", color.AsUssString());
            public USSBuilder Font(string path) => WithProperty("-unity-font", $"url(\"{path.Replace("\"", "\\\"")}\")");
            public USSBuilder FontDefinition(string path) => WithProperty("-unity-font", $"url(\"{path.Replace("\"", "\\\"")}\")");
            public USSBuilder FontStyle(StyleEnum<FontStyle> style) => WithProperty("-unity-font-style", style.AsUssString());
            public USSBuilder OverflowClipBox(StyleEnum<OverflowClipBox> clipBox) => WithProperty("-unity-overflow-clip-box", clipBox.AsUssString());
            public USSBuilder ParagraphSpacing(StyleLength length) => WithProperty("-unity-paragraph-spacing", length.AsUssString());
            public USSBuilder SliceBottom(StyleInt value) => WithProperty("-unity-slice-bottom", value.AsUssString());
            public USSBuilder SliceLeft(StyleInt value) => WithProperty("-unity-slice-left", value.AsUssString());
            public USSBuilder SliceRight(StyleInt value) => WithProperty("-unity-slice-right", value.AsUssString());
            public USSBuilder SliceScale(StyleFloat value) => WithProperty("-unity-slice-scale", value.AsUssString());
            public USSBuilder SliceTop(StyleInt value) => WithProperty("-unity-slice-top", value.AsUssString());
            public USSBuilder TextAlign(StyleEnum<TextAnchor> anchor) => WithProperty("-unity-text-align", anchor.AsUssString());
            public USSBuilder TextOutlineColor(StyleColor color) => WithProperty("-unity-text-outline-color", color.AsUssString());
            public USSBuilder TextOutlineWidth(StyleFloat value) => WithProperty("-unity-text-outline-width", value.AsUssString());
            public USSBuilder TextOverflowPosition(StyleEnum<TextOverflowPosition> overflowPosition) => WithProperty("-unity-text-overflow-position", overflowPosition.AsUssString());
            public USSBuilder Visibility(StyleEnum<Visibility> visibility) => WithProperty("visibility", visibility.AsUssString());
            public USSBuilder WhiteSpace(StyleEnum<WhiteSpace> whiteSpace) => WithProperty("white-space", whiteSpace.AsUssString());
            public USSBuilder Width(StyleLength length) => WithProperty("width", length.AsUssString());
            public USSBuilder WordSpacing(StyleLength length) => WithProperty("word-spacing", length.AsUssString());

            private USSBuilder WithProperty(string name, string value)
            {
                if (value != null)
                    properties[name] = value;

                return this;
            }
        }

        public static USSBuilder Style => new();
        public static USSBuilder NamedStyle(string name) => new(name);

        public string Selector => $".{ClassName}";
        public string ClassName => className ?? GetClassName();
        private string className;
        public string UssText => ussText ?? GetUssText();
        private string ussText;

        private SortedDictionary<string, string> properties;
        private SortedDictionary<string, USS> nested;

        public USS(string className, SortedDictionary<string, string> properties, SortedDictionary<string, USS> nested)
        {
            this.className = className;
            this.properties = new(properties);
            this.nested = new(nested);
        }

        public override string ToString() => ClassName;
        
        public static implicit operator string(USS uss) => uss.ToString();
        
        public string GetClassName()
        {
            using var sha256 = SHA256.Create();

            var builder = new StringBuilder();

            foreach (var child in nested)
                builder.Append(child.Key).Append(":").Append(child.Value.ClassName).Append("\n");
            
            builder.AppendLine("@");
            
            foreach (var property in properties)
                builder.Append(property.Key).Append(":").Append(property.Value).Append('\n');
            
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(builder.ToString()));

            uint hash = BitConverter.ToUInt32(hashBytes, 0);

            return $"s{Sanitize(hash.ToString())}";
        }

        public string GetUssText(string overrideSelector = null)
        {
            var builder = new StringBuilder();

            overrideSelector = overrideSelector?.Trim();

            if (properties.Count > 0)
            {
                builder.Append(overrideSelector ?? Selector);
                builder
                    .Append(" {\n")
                    .AppendJoin('\n', properties.Select(entry => $"\t{entry.Key}: {entry.Value};"))
                    .Append("\n}");
            }

            if (nested.Count <= 0) return builder.ToString();

            string prefix = $"{overrideSelector ?? ""}{Selector}";

            foreach (var nestedSelector in nested)
            {
                var nestedSelectors = nestedSelector.Key.Replace("&", prefix).Split(",").Select(x => x.Trim());
                
                foreach (string nestedSplitSelector in nestedSelectors)
                    builder.Append('\n').Append(nestedSelector.Value.GetUssText(nestedSplitSelector));
            }

            return builder.ToString();
        }
        
        private static string Sanitize(string value) => value.Replace("-", "_");
    }

    //TODO: change to proper cursor type and handle resource links
    [PublicAPI] public enum CursorType
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
    
    public struct StyleAngle: IStyleValue<Angle>
    {
        public Angle value { get; set; }
        public StyleKeyword keyword { get; set; }
        
        public static implicit operator StyleAngle(Angle angle) => new() { value = angle, keyword = StyleKeyword.Undefined };
        public static implicit operator StyleAngle(StyleKeyword keyword) => new() { value = Angle.Radians(0), keyword = keyword };
    }

    [PublicAPI] public struct Transition
    {
        private readonly float duration;
        private readonly float delay;
        private readonly string property;
        private readonly EasingMode easingFunction;

        public Transition(string property, float duration = 1, float delay = 0, EasingMode easingFunction = EasingMode.Ease)
        {
            this.property = property;
            this.duration = duration;
            this.delay = delay;
            this.easingFunction = easingFunction;
        }
        
        public override string ToString() => $"{property} {duration} {delay} {USSExtensions.ToUssString(easingFunction)}";
    }
    
    [PublicAPI, SuppressMessage("ReSharper", "StaticMemberInGenericType")]
    public static class USSExtensions {
        private static class EnumConverter<T> where T : Enum
        {
            public static string AsUssString(T value) => valueCache[value] ?? defaultValue;
            
            private static readonly Dictionary<T, string> valueCache = GenerateCache();
            private static readonly string defaultValue = valueCache.First().Value;

            private static Dictionary<T, string> GenerateCache() =>
                Enum.GetValues(typeof(T)).Cast<T>().ToDictionary(value => value, value => NameToString(value.ToString()));
        }

        private static readonly MatchEvaluator namePartEvaluator = match => $"{match.Value[0]}_{match.Value[1].ToString().ToLowerInvariant()}";

        private static string NameToString(string name) => Regex.Replace(name, "[^A-Z][A-Z]", namePartEvaluator);
        
        private static string AsUssString<T>(this IStyleValue<T> value, Func<T, string> converter) => value.keyword switch
        {
            StyleKeyword.Undefined => converter(value.value),
            StyleKeyword.Auto => "auto",
            StyleKeyword.None => "none",
            StyleKeyword.Initial => "initial",
            _ => "auto"
        };
        
        public static string AsUssString(this IStyleValue<int> value) =>
            value.AsUssString(v => v.ToString(CultureInfo.InvariantCulture.NumberFormat));
        public static string AsUssString(this IStyleValue<float> value) =>
            value.AsUssString(v => v.ToString(CultureInfo.InvariantCulture.NumberFormat));

        public static string ToUssString(Length length) => length.ToString();
        public static string AsUssString(this StyleLength value) => value.AsUssString(ToUssString);
        
        public static string ToUssString<T>(T value) where T : struct, Enum, IConvertible => EnumConverter<T>.AsUssString(value);
        public static string AsUssString<T>(this StyleEnum<T> value) where T : struct, Enum, IConvertible => value.AsUssString(ToUssString);
        
        public static string ToUssString(Color color) =>
            $"rgba({color.r * 255:0}, {color.g * 255:0}, {color.b * 255:0}, {color.a:0.#})";
        public static string AsUssString(this StyleColor color) => color.AsUssString(ToUssString);
        
        public static string ToUssString(Scale scale) => $"{scale.value.x} ${scale.value.y}";
        public static string AsUssString(this StyleScale scale) => scale.AsUssString(ToUssString);
        
        public static string ToUssString(Angle angle) => angle.ToString();
        public static string AsUssString(this StyleAngle angle) => angle.AsUssString(ToUssString);
        
        public static string ToUssString(BackgroundRepeat repeat) => $"{ToUssString(repeat.x)} {ToUssString(repeat.y)}";
        public static string AsUssString(this StyleBackgroundRepeat repeat) => repeat.AsUssString(ToUssString);
        
        public static string ToUssString(BackgroundPosition position) => $"{ToUssString(position.keyword)} {ToUssString(position.offset)}";
        public static string AsUssString(this StyleBackgroundPosition position) => position.AsUssString(ToUssString);
    }
}