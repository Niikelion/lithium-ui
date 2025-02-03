using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace UI.Li.USS
{
    [CreateAssetMenu(fileName = "UssSettings.asset", menuName = "Lithium/UssSettings")]
    public class USSStylesheetSettingsScriptableObject: ScriptableObject
    {
        public string[] IncludeNamespaces;
        public string FileName;
        
        public string Generate()
        {
            var extractedUss = ExtractAllUSS(IncludeNamespaces);
            
            return string.Join("\n\n", extractedUss.Select(uss => uss.UssText));
        }

        private static IEnumerable<USS> ExtractAllUSS(string[] includedNamespaces)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!includedNamespaces.Contains(type.Namespace))
                        continue;
                    
                    var fields = type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic).Where(field => field.FieldType == typeof(USS));
                    foreach (var field in fields)
                        yield return field.GetValue(null) as USS;
                }
            }
        }
    }
}