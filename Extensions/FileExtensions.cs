using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Assets.Scripts.Framework.Extensions
{
    public static class FileExtensions
    {
        /// <summary>
        /// Loads all assets at path
        /// </summary>
        /// <param name="path"> Path to folder </param>
        /// <typeparam name="T"> Asset type </typeparam>
        /// <returns></returns>
        public static HashSet<T> LoadAllAssetsAtPath<T>(this string path) where T : UnityEngine.Object
        {
            var filePaths = Directory.GetFiles(path);
            HashSet<T> objects = new(); 

            foreach (var filePath in filePaths)
            {
                var file = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(filePath);
                
                if (file is not null)
                {
                    objects.Add(file);
                }
            }

            return objects;
        }
        
        /// <summary>
        /// Creates a C# file
        /// </summary>
        /// <param name="filePath">Filepath of the C# file</param>
        /// <param name="fileName">Name for the C# file</param>
        /// <returns>Filepath to C# file</returns>
        public static string CreateCSharpFile(string filePath, string fileName)
        {
            var fileNameWithType = fileName + ".cs";
            var path = filePath + fileNameWithType;
            
            using (File.Create(path)) { }
            
            return path;
        }

        // This
        public static void ReplaceTextInFile(string filePath, string oldText, string newText)
        {
            var fileText = File.ReadAllText(filePath);
            fileText = fileText.Replace(oldText, newText);
            File.WriteAllText(filePath, fileText);
        }

        /// <summary>
        /// Write text to file
        /// </summary>
        /// <param name="filePath"> Path to file </param>
        /// <param name="text"> Byte array to write into file </param>
        /// <param name="fileMode"> Filemode to open file in </param>
        public static void WriteTextToFile(string filePath, byte[] text, FileMode fileMode = FileMode.Append)
        {
            using (var fs = File.Open(filePath, fileMode))
            {
                fs.Write(text, 0, text.Length);
            }
        }
        
        public static void WriteString(this FileStream fs, string str)
        {
            var bytes = new UTF8Encoding(true).GetBytes(str);
            fs.Write(bytes, 0, bytes.Length);
        }
    }
}
