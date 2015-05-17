using System;

namespace MindHelper
{
    public class StringFormatter
    {
        public static string RemoveWhiteSpace(string Value)
        {
            return Value.Replace(" ", string.Empty);
        }
    }
}
