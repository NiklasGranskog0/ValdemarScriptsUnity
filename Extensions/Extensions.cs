using System.Collections.Generic;
using System.IO;

namespace Assets.Scripts.Framework.Extensions
{
    public static class Extensions
    {
        public static string Color(this string s, string color) => $"<color={color.ToUpper()}>{s}</color>";

        
    }
}