using System;
using System.Collections.Generic;
using System.Linq;

namespace MindHelper.Math
{
    public static class GCF
    {
        public static int Get(int[] Numbers)
        {
            return GetCommonFactors(Numbers).Last();
        }

        public static List<int> GetCommonFactors(int[] Numbers, List<int> CommonFactors = null, Dictionary<int, List<int>> Factors = null, int[] NextNumbers = null)
        {
            // instantiates null variables
            if (CommonFactors == null || Factors == null || NextNumbers == null)
            {
                CommonFactors = new List<int>();
                NextNumbers = new int[Numbers.Length];
                Factors = new Dictionary<int, List<int>>();
                for (int i = 0; i < Numbers.Length; i++)
                {
                    Factors.Add(i, new List<int>());
                    NextNumbers[i] = 1;
                }
            }

            // Checks if factorable
            for (int i = 0; i < Numbers.Length; i++)
            {
                if (IsFactorable(Numbers[i], NextNumbers[i]))
                {
                    Factors[i].Add(NextNumbers[i]);
                }
            }

            // check for common factors
            if (CollectionUtility.CollectionItemContainsTheValue<int>(Factors[0].Last(), Factors.Values.ToList()))
            {
                CommonFactors.Add(Factors[0].Last());
            }

            // return the common factors found if one of the numbers reach end of factors.
            for (int i = 0; i < Numbers.Length; i++)
            {
                NextNumbers[i]++;
                if (NextNumbers[i] > Numbers[i])
                { return CommonFactors; }
                else
                { continue; }
            }

            // if one of the numbers not yet reach end of factors then continue getting the other factors.
            return GetCommonFactors(Numbers, CommonFactors, Factors, NextNumbers);
            
        }

        private static bool IsFactorable(int FactorBy, int Number)
        {
            return (FactorBy % Number == 0);
        }
    }
}
