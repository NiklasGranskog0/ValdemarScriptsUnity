using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace Assets.Scripts.Framework.Extensions
{
    public static class UnityIcon
    {
        public static Texture2D LoadIcon(string path) => EditorGUIUtility.Load(path) as Texture2D;

        // https://github.com/halak/unity-editor-icons
        public const string k_ErrorIcon = "d_console.erroricon";
        public const string k_ErrorIconSmall = "d_console.erroricon.sml";
        public const string k_ErrorIconSmallInactive = "d_console.erroricon.inactive.sml";
        public const string k_WarningIcon = "d_console.warnicon";
        public const string k_WarningIconSmall = "d_console.warnicon.sml";
        public const string k_WarningIconSmallInactive = "d_console.warnicon.inactive.sml";
        public const string k_MessageIcon = "d_console.infoicon";
        public const string k_MessageIconSmall = "d_console.infoicon.sml";
        public const string k_MessageIconSmallInactive = "d_console.infoicon.inactive.sml";
        public const string k_ConsoleIcon = "d_UnityEditor.ConsoleWindow";

        public const string k_AvatarBlendBackground = "d_AvatarBlendBackground";
        public const string k_StacktraceBackground = "builtin skins/darkskin/images/projectbrowsericonareabg.png";
        public const string k_BackgroundOdd = "builtin skins/darkskin/images/cn entrybackodd.png";
        public const string k_BackgroundEven = "builtin skins/darkskin/images/cnentrybackeven.png";
        public const string k_BackgroundSelected = "builtin skins/darkskin/images/menuitemhover.png";

        // [MenuItem("Unity Editor Icons/Export All")]
        private static void ExportIcons()
        {
            EditorUtility.DisplayProgressBar("Export Icons", "Exporting...", 0.0f);

            try
            {
                var editorAssetBundle = GetEditorAssetBundle();
                var iconsPath = UnityEditor.Experimental.EditorResources.iconsPath;
                var count = 0;

                foreach (var assetName in EnumerableIcons(editorAssetBundle, iconsPath))
                {
                    var icon = editorAssetBundle.LoadAsset<Texture2D>(assetName);
                    if (icon == null) continue;
                    if (icon.format == TextureFormat.DXT5) continue;

                    var readableTexture = new Texture2D(icon.width, icon.height, icon.format, icon.mipmapCount > 1);
                    Graphics.CopyTexture(icon, readableTexture);

                    var folderPath =
                        Path.GetDirectoryName(Path.Combine("icons/original/", assetName[iconsPath.Length..]));

                    if (Directory.Exists(folderPath) == false)
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    var iconPath = Path.Combine(folderPath, icon.name + ".png");
                    File.WriteAllBytes(iconPath, readableTexture.EncodeToPNG());

                    count++;
                }

                Debug.Log($"{count} icons has been exported!");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private static IEnumerable<string> EnumerableIcons(AssetBundle editorAssetBundle, string iconsPath)
        {
            foreach (var assetName in editorAssetBundle.GetAllAssetNames())
            {
                if (assetName.StartsWith(iconsPath, StringComparison.OrdinalIgnoreCase) == false) continue;
                if (assetName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) == false &&
                    assetName.EndsWith(".asset", StringComparison.OrdinalIgnoreCase) == false) continue;

                yield return assetName;
            }
        }

        private static AssetBundle GetEditorAssetBundle()
        {
            var editorGUIUtility = typeof(EditorGUIUtility);
            var getEditorAssetBundle =
                editorGUIUtility.GetMethod("GetEditorAssetBundle", BindingFlags.NonPublic | BindingFlags.Static);
            return (AssetBundle)getEditorAssetBundle.Invoke(null, new object[] { });
        }
    }
}
#endif