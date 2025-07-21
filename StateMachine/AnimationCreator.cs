using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Framework.Extensions;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Framework.StateMachine
{
    public enum BodyLayer
    {
        UpperBody,
        LowerBody
    }

    public class AnimationCreator : MonoBehaviour // MonoBehaviour temp
    {
        [Header("Change to animation object(unit)")]
        // [SerializeField] private string animationClipsPath;
        [SerializeField]
        private AnimationClip entryIdleAnimationClip;

        [SerializeField] private Object model;

        [Header("Don't Change")] private const string k_PathToEnumFolder = "Assets/Assets/Scripts/Framework/Enums/";
        private const string k_PathToEnumTemplateClass =
            "Assets/Assets/Animations/Characters/CreatorTester/EnumClassTemplate.cs";
        private const string k_PathToAnimationDataFile =
            "Assets/Assets/Scripts/ScriptableObjectsScripts/AnimationData.cs";

        private const int k_LowerBody = (int)BodyLayer.LowerBody;
        private const int k_UpperBody = (int)BodyLayer.UpperBody;

        public void Awake()
        {
            string modelName = model.name;
            modelName = modelName.Replace(" ", "_");

            var controller = new AnimatorController()
            {
                name = $"{modelName}_AnimatorController"
            };

            // TODO: Seems like it's not possible to set avatar masks ?

            if (AssetDatabase.GetAssetPath(controller) is not (null or "")) return;

            AssetDatabase.CreateAsset(controller,
                $"Assets/Assets/Animations/Characters/CreatorTester/{controller.name}.asset");
            var allObjects = AssetDatabase.LoadAllAssetsAtPath(AssetDatabase.GetAssetPath(model));

            var allClips = new HashSet<AnimationClip>();

            foreach (var obj in allObjects)
            {
                if (obj.name.Contains("preview")) continue;
                
                if (obj.name.Contains('.'))
                {
                    obj.name = obj.name.Replace(".", "");
                }
                
                if (obj is AnimationClip clip)
                {
                    allClips.Add(clip);
                }
            }

            controller.AddLayer("Upper Body");
            controller.AddLayer("Lower Body");

            // Set idle entry animation in layers
            controller.AddMotion(entryIdleAnimationClip, k_LowerBody);
            controller.AddMotion(entryIdleAnimationClip, k_UpperBody);

            // Add all animation clips, create a animator has for each clip, (save clip name maybe)
            int[] stateHashArray = new int[allClips.Count];
            string[] nameArray = new string[allClips.Count];
            int count = 0;
            foreach (var clip in allClips)
            {
                controller.AddMotion(clip, k_UpperBody);
                controller.AddMotion(clip, k_LowerBody);

                stateHashArray[count] = Animator.StringToHash(clip.name);
                nameArray[count] = clip.name;
                count++;
            }

            // Create enum for all clips
            var fileName = model.name.Replace("_", "");

            if (fileName.Contains(' '))
            {
                fileName = fileName.Replace(" ", "");
            }

            if (File.Exists($"{k_PathToEnumFolder}{fileName}Animations.cs")) return;

            File.Copy(k_PathToEnumTemplateClass, $"{k_PathToEnumFolder}{fileName}Animations.cs");
            FileExtensions.ReplaceTextInFile($"{k_PathToEnumFolder}{fileName}Animations.cs", "{fileName}",
                $"{fileName}");
            FileExtensions.ReplaceTextInFile($"{k_PathToEnumFolder}{fileName}Animations.cs", "//", "");

            int index;
            using (var sr = File.OpenText($"{k_PathToEnumFolder}{fileName}Animations.cs"))
            {
                var text = sr.ReadToEnd();
                index = text.LastIndexOf('{') + 1;
            }

            using (var fs = File.Open($"{k_PathToEnumFolder}{fileName}Animations.cs", FileMode.Open))
            {
                fs.Position = index;

                foreach (var clipName in nameArray)
                {
                    var nameToInsert = clipName.Replace(" ", "");

                    nameToInsert = ReplaceNumberInFirstCharacter(nameToInsert);

                    fs.WriteString(nameToInsert + ",\n\t\t");
                }

                const string closingBracket = "}";
                fs.WriteString(closingBracket + "\n" + closingBracket);
            }

            int endIndex;
            using (var sr = File.OpenText(k_PathToAnimationDataFile))
            {
                var text = sr.ReadToEnd();
                endIndex = text.LastIndexOf(';') + 1;
            }

            using (var fs = File.Open(k_PathToAnimationDataFile, FileMode.Open))
            {
                fs.Position = endIndex;
                var start = $"\n\nprivate readonly int[] m_{fileName} =" + "\n\t\t{\n\t\t\t";
                var end = "};\n\t}\n}";

                fs.WriteString(start);

                foreach (var stateHash in stateHashArray)
                {
                    fs.WriteString($"{stateHash},\n\t\t\t");
                }

                fs.WriteString(end);
            }

            AssetDatabase.Refresh();
        }

        private string ReplaceNumberInFirstCharacter(string stringToEdit) // regex ? lol
        {
            var numberToReplace = new[] { '1', '2', '3', '4', '5', '6', '7', '9', '0' };
            var numberToString = new[]
                { "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Zero" };

            for (int i = 0; i < numberToReplace.Length; i++)
            {
                if (stringToEdit.Contains(numberToReplace[i]) && stringToEdit.IndexOf(numberToReplace[i]) == 0)
                {
                    stringToEdit = stringToEdit.Replace(numberToReplace[i].ToString(), numberToString[i]);
                }

                if (stringToEdit.Contains('|'))
                {
                    stringToEdit = stringToEdit.Replace('|', '_');
                }
                
                if (stringToEdit.Contains('&'))
                {
                    stringToEdit = stringToEdit.Replace('&', '_');
                }
                
                if (stringToEdit.Contains('!'))
                {
                    stringToEdit = stringToEdit.Replace('!', '_');
                }
                
                if (stringToEdit.Contains('.'))
                {
                    stringToEdit = stringToEdit.Replace('.', '_');
                }
            }

            return stringToEdit;
        }
    }
}