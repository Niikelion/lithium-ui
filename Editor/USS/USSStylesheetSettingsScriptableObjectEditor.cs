using System.IO;
using System.Linq;
using UI.Li.USS;
using UnityEditor;
using UnityEngine.Rendering;
using static UI.Li.Common.Common;
using static UI.Li.Common.Layout.Layout;
using static UI.Li.Editor.Fields;

namespace UI.Li.Editor.USS
{
    [CustomEditor(typeof(USSStylesheetSettingsScriptableObject))]
    public class USSStylesheetSettingsScriptableObjectEditor: ComposableEditor
    {
        protected override IComponent Layout() => Col(Inspector(this), Button(OnGenerate, "Generate"));

        private void OnGenerate()
        {
            var generator = target as USSStylesheetSettingsScriptableObject;

            if (generator == null)
                return;
            
            var currentPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(generator));

            if (currentPath == null)
                return;
            
            var stylesheetPath = Path.Combine(currentPath, generator.FileName.Length > 0 ? $"{generator.FileName}.uss" : "stylesheet.uss");
            
            var source = generator.Generate();
            
            File.WriteAllText(stylesheetPath, source);
            AssetDatabase.Refresh();
        }
    }

    [InitializeOnLoad]
    public static class USSGenerator
    {
        static USSGenerator()
        {
            AssemblyReloadEvents.afterAssemblyReload += RegenerateStyles;
        }

        private static void RegenerateStyles()
        {
            var assetGuids = AssetDatabase.FindAssets("");
            var assetPaths = assetGuids.Select(AssetDatabase.GUIDToAssetPath);
            var assets = assetPaths.Select(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>);
            
            foreach (var asset in assets)
            {
                if (asset is not USSStylesheetSettingsScriptableObject generator)
                    continue;
                
                GenerateStyleFile(generator);
            }

            AssetDatabase.Refresh();
        }
        
        private static void GenerateStyleFile(USSStylesheetSettingsScriptableObject generator)
        {
            if (generator == null)
                return;
            
            var currentPath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(generator));

            if (currentPath == null)
                return;
            
            var stylesheetPath = Path.Combine(currentPath, generator.FileName.Length > 0 ? $"{generator.FileName}.uss" : "stylesheet.uss");
            
            var source = generator.Generate();
            
            File.WriteAllText(stylesheetPath, source);
        }
    }
}