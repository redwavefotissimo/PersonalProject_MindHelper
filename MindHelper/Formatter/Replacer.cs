using System;
using System.Collections.Generic;

namespace MindHelper.Formatter
{
    public class Replacer
    {
        public delegate string _ReplacerFunction(params object[] Parameters);
        private static Dictionary<string, _ReplacerFunction> ReplacerFunctionList;

        static Replacer()
        {
            ReplacerFunctionList = new Dictionary<string, _ReplacerFunction>();
        }

        public static void AddReplacerFunction(string LookupString, _ReplacerFunction ReplacerFunction)
        {
            ReplacerFunctionList.Add(LookupString, ReplacerFunction);
        }

        public static string Replace(string StringToBeSearch, string LookupString, params object[] Parameters)
        {
            return StringToBeSearch.Replace(LookupString, ReplacerFunctionList[LookupString](Parameters));
        }
    }
}
