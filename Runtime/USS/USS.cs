using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

        public USS Color(StyleColor color) => WithColorProperty("color", color);

        private USS WithColorProperty(string key, StyleColor value) => WithProperty(key, value, ColorToString);
        
        private USS WithProperty<T>(string key, T value, [NotNull] Func<T, string> converter) => WithProperty(key, converter(value));
        
        private USS WithProperty(string name, string value)
        {
            var newUSS = new USS(this);
            
            if (value != null)
                newUSS.properties["color"] = value;
            
            return newUSS;
        }
        
        private static string ColorToString(StyleColor color) => $"rgba({color.value.r*255:0}, {color.value.g*255:0}, {color.value.b*255:0}, {color.value.a:0.#})";
        
        private static string Sanitize(string value) => value.Replace("-", "_");
    }
}