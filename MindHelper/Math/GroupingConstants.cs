using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MindHelper.Math
{
    public static class GroupingConstants
    {
        public const string OpenParenthesis = "(";

        public const string CloseParenthesis = ")";

        public const string OpenBracket = "[";

        public const string CloseBracket = "]";

        public const string OpenBrace = "{";

        public const string CloseBrace = "}";

        public static string[] OpenGroupingList = new string[] { OpenParenthesis, OpenBracket, OpenBrace };

        public static string[] CloseGroupingList = new string[] { CloseParenthesis, CloseBracket, CloseBrace };
    }
}
