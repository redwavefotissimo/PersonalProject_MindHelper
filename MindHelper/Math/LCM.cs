using System;
using System.Collections.Generic;
using System.Linq;

namespace MindHelper.Math
{
    public static class LCM
    {
        public static int Get(int[] Numbers)
        {
            Array.Sort(Numbers);
            return GetMultiples(Numbers);
        }

        private static int GetMultiples(int[] Numbers, Dictionary<int, List<int>> Multiples = null, int Index = 1)
        {
            // instantiate null variables.
            if (Multiples == null)
            {
                Multiples = new Dictionary<int, List<int>>();
                for (int i = 0; i < Numbers.Length; i++)
                {
                    Multiples.Add(i, new List<int>());
                }
            }

            for (int i = 0; i < Numbers.Length; i++)
            {
                Multiples[i].Add(Numbers[i] * Index);
            }

            if (CollectionUtility.CollectionItemContainsTheValue<int>(Multiples[0].Last(), Multiples.Values.ToList()))
            {
                return Multiples[0].Last();
            }

            Index++;

            return GetMultiples(Numbers, Multiples, Index);
        }
    }
}
