using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MindHelper
{
    public class Converter
    {
        public enum Types
        {
            Int16,
            Int32,
            Int64,
            Decimal,
            Double,
            DateTime,
            Boolean,
            String
        }

        /// <summary>
        /// Converts from 1 data type to another.
        /// </summary>
        /// <param name="Value">object to be converted.</param>
        /// <param name="Type">type of data to convert into.</param>
        /// <returns>Converted object.</returns>
        public static object ConvertTo(object Value, Types Type)
        {
            switch (Type)
            {
                case Types.Int16:
                    Value = Convert.ToInt16(Value);
                    break;
                case Types.Int32:
                    Value = Convert.ToInt32(Value);
                    break;
                case Types.Int64:
                    Value = Convert.ToInt64(Value);
                    break;
                case Types.Decimal:
                    Value = Convert.ToDecimal(Value);
                    break;
                case Types.Double:
                    Value = Convert.ToDouble(Value);
                    break;
                case Types.DateTime:
                    Value = Convert.ToDateTime(Value);
                    break;
                case Types.Boolean:
                    Value = Convert.ToBoolean(Value);
                    break;
                case Types.String:
                    Value = Convert.ToString(Value);
                    break;
            }

            return Value;
        }

    }
}
