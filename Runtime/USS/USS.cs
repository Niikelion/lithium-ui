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
        public USS BackgroundColor(StyleColor color) => WithProperty("background-color", color, ColorToString);
        //StyleBackground BackgroundImage
        //StyleBackgroundPosition BackgroundPositionX
        //StyleBackgroundPosition BackgroundPositionY
        //StyleBackgroundRepeat BackgroundRepeat
        //StyleBackgroundSize BackgroundSize
        public USS BorderBottomColor(StyleColor color) => WithProperty("border-bottom-color", color, ColorToString);
        public USS BorderBottomLeftRadius(StyleLength length) => WithProperty("border-bottom-left-radius", length, LengthToString);
        public USS BorderBottomRightRadius(StyleLength length) => WithProperty("border-bottom-right-radius", length, LengthToString);
        public USS BorderBottomWidth(StyleFloat value) => WithProperty("border-bottom-width", value, FloatToString);
        public USS BorderLeftColor(StyleColor color) => WithProperty("border-left-color", color, ColorToString);
        public USS BorderLeftWidth(StyleFloat value) => WithProperty("border-left-width", value, FloatToString);
        public USS BorderRightColor(StyleColor color) => WithProperty("border-right-color", color, ColorToString);
        public USS BorderRightWidth(StyleFloat value) => WithProperty("border-right-width", value, FloatToString);
        public USS BorderTopColor(StyleColor color) => WithProperty("border-top-color", color, ColorToString);
        public USS BorderTopLeftRadius(StyleLength length) => WithProperty("border-top-left-radius", length, LengthToString);
        public USS BorderTopRightRadius(StyleLength length) => WithProperty("border-top-right-radius", length, LengthToString);
        public USS BorderTopWidth(StyleFloat value) => WithProperty("border-top-width", value, FloatToString);
        public USS Bottom(StyleLength length) => WithProperty("bottom", length, LengthToString);
        public USS Color(StyleColor color) => WithColorProperty("color", color);
        //StyleCursor Cursor
        public USS Display(StyleEnum<DisplayStyle> style) => WithProperty("display", style, DisplayToString);
        public USS FlexBasis(StyleLength length) => WithProperty("flex-basis", length, LengthToString);
        //StyleEnum<FlexDirection> FlexDirection
        public USS FlexGrow(StyleFloat value) => WithProperty("flex-grow", value, FloatToString);
        public USS FlexShrink(StyleFloat value) => WithProperty("flex-shrink", value, FloatToString);
        //StyleEnum<Wrap> FlexWrap
        public USS FontSize(StyleLength length) => WithProperty("font-size", length, LengthToString);
        public USS Height(StyleLength length) => WithProperty("height", length, LengthToString);
        //StyleEnum<Justify> JustifyContent
        public USS Left(StyleLength length) => WithProperty("left", length, LengthToString);
        public USS LetterSpacing(StyleLength length) => WithProperty("letter-spacing", length, LengthToString);
        public USS MarginBottom(StyleLength length) => WithProperty("margin-bottom", length, LengthToString);
        public USS MarginLeft(StyleLength length) => WithProperty("margin-left", length, LengthToString);
        public USS MarginRight(StyleLength length) => WithProperty("margin-right", length, LengthToString);
        public USS MarginTop(StyleLength length) => WithProperty("margin-top", length, LengthToString);
        public USS MaxHeight(StyleLength length) => WithProperty("max-height", length, LengthToString);
        public USS MaxWidth(StyleLength length) => WithProperty("max-width", length, LengthToString);
        public USS MinHeight(StyleLength length) => WithProperty("min-height", length, LengthToString);
        public USS MinWidth(StyleLength length) => WithProperty("min-width", length, LengthToString);
        public USS Opacity(StyleFloat value) => WithProperty("opacity", value, FloatToString);
        //StyleEnum<Overflow> Overflow
        public USS PaddingBottom(StyleLength length) => WithProperty("padding-bottom", length, LengthToString);
        public USS PaddingLeft(StyleLength length) => WithProperty("padding-left", length, LengthToString);
        public USS PaddingRight(StyleLength length) => WithProperty("padding-right", length, LengthToString);
        public USS PaddingTop(StyleLength length) => WithProperty("padding-top", length, LengthToString);
        //StyleEnum<Position> Position
        public USS Right(StyleLength length) => WithProperty("right", length, LengthToString);
        //StyleRotate Rotate
        //StyleScale Scale
        //StyleEnum<TextOverflow> TextOverflow
        //StyleTextShadow TextShadow
        public USS Top(StyleLength length) => WithProperty("top", length, LengthToString);
        //StyleTransformOrigin TransformOrigin
        //StyleList<TimeValue> TransitionDelay
        //StyleList<TimeValue> TransitionDuration
        //StyleList<StylePropertyName> TransitionProperty
        //StyleList<EasingFunction> TransitionTimingFunction
        //StyleTranslate Translate
        public USS BackgroundImageTintColor(StyleColor color) => WithProperty("unity-background-image-tint-color", color, ColorToString);
        //StyleFont Font
        //StyleFontDefinition UnityFontDefinition
        //StyleEnum<FontStyle> UnityFontStyleAndWeight
        //StyleEnum<OverflowClipBox> UnityOverflowClipBox
        public USS ParagraphSpacing(StyleLength length) => WithProperty("unity-paragraph-spacing", length, LengthToString);
        public USS SliceBottom(StyleInt value) => WithProperty("unity-slice-bottom", value, IntToString);
        public USS SliceLeft(StyleInt value) => WithProperty("unity-slice-left", value, IntToString);
        public USS SliceRight(StyleInt value) => WithProperty("unity-slice-right", value, IntToString);
        public USS SliceScale(StyleFloat value) => WithProperty("unity-slice-scale", value, FloatToString);
        public USS SliceTop(StyleInt value) => WithProperty("unity-slice-top", value, IntToString);
        //StyleEnum<TextAnchor> UnityTextAlign
        public USS TextOutlineColor(StyleColor color) => WithProperty("unity-text-outline-color", color, ColorToString);
        public USS TextOutlineWidth(StyleFloat value) => WithProperty("unity-text-outline-width", value, FloatToString);
        //StyleEnum<TextOverflowPosition> UnityTextOverflowPosition
        //StyleEnum<Visibility> Visibility
        //StyleEnum<WhiteSpace> WhiteSpace
        public USS Width(StyleLength length) => WithProperty("width", length, LengthToString);
        public USS WordSpacing(StyleLength length) => WithProperty("word-spacing", length, LengthToString);

        private USS WithColorProperty(string key, StyleColor value) =>
            WithProperty(key, value, ColorToString);
        
        private USS WithProperty<T>(string key, IStyleValue<T> value, [NotNull] Func<T, string> converter) =>
            WithProperty(key, ValueToString(value, converter));

        private USS WithProperty(string name, string value)
        {
            var newUSS = new USS(this);
            
            if (value != null)
                newUSS.properties["color"] = value;
            
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
}