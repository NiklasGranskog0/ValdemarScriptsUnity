using System;
using System.Collections.Generic;

namespace Assets.Scripts.Framework.Extensions
{
    public static class ListExtensions
    {
        private static Random s_rng;

        public static IList<T> Shuffle<T>(this IList<T> list)
        {
            if (s_rng == null) s_rng = new Random();
            var count = list.Count;

            while (count > 1)
            {
                --count;
                var index = s_rng.Next(count + 1);
                (list[index], list[count]) = (list[count], list[index]);
            }

            return list;
        }
    }
}
