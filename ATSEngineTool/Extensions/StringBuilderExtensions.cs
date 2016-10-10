using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    public static class StringBuilderExtensions
    {
        /// <summary>
        /// Clears the current contents of this StringBuilder
        /// </summary>
        /// <param name="builder"></param>
        public static void Clear(this StringBuilder builder)
        {
            builder.Length = 0;
        }

        /// <summary>
        /// Appends an Object to this string builder if the <paramref name="condition"/> is true.
        /// </summary>
        /// <param name="condition">Indicates whether we append this object to the end of this StringBuilder</param>
        /// <param name="value">The value to append</param>
        public static StringBuilder AppendIf(this StringBuilder builder, bool condition, object value)
        {
            if (condition)
                builder.Append(value);
            return builder;
        }

        /// <summary>
        /// Appends a string to this string builder if the <paramref name="condition"/> is true, followed by a line terminator.
        /// </summary>
        /// <param name="condition">Indicates whether we append this object to the end of this StringBuilder</param>
        /// <param name="value">The value to append</param>
        public static StringBuilder AppendLineIf(this StringBuilder builder, bool condition, string value)
        {
            if (condition)
                builder.AppendLine(value);
            return builder;
        }

        /// <summary>
        /// Appends a string to this string builder if the <paramref name="condition"/> is true, followed by a line terminator.
        /// </summary>
        /// <param name="condition">Indicates whether we append this object to the end of this StringBuilder</param>
        /// <param name="value">The value to append</param>
        public static StringBuilder AppendLineIf(this StringBuilder builder, bool condition, string trueValue, string falseValue)
        {
            if (condition)
                builder.AppendLine(trueValue);
            else
                builder.AppendLine(falseValue);
            return builder;
        }

        /// <summary>
        /// Appends a new line to this string builder if the <paramref name="condition"/> is true
        /// </summary>
        /// <param name="condition">Indicates whether we append a newline to the end of this StringBuilder</param>
        public static StringBuilder AppendLineIf(this StringBuilder builder, bool condition)
        {
            if (condition)
                builder.AppendLine();
            return builder;
        }

        /// <summary>
        /// Appends the specified character if the <paramref name="condition"/> is true
        /// </summary>
        /// <param name="condition">Indicates whether we append a newline to the end of this StringBuilder</param>
        public static StringBuilder AppendIf(this StringBuilder builder, bool condition, char c, int repeatCount = 1)
        {
            if (condition)
                builder.Append(c, repeatCount);
            return builder;
        }

        /// <summary>
        /// Appends the specified string if the <paramref name="condition"/> is true
        /// </summary>
        /// <param name="condition">Indicates whether we append a newline to the end of this StringBuilder</param>
        public static StringBuilder AppendIf(this StringBuilder builder, bool condition, string value, int repeatCount = 1)
        {
            if (condition)
                builder.Append(value, repeatCount);
            return builder;
        }

        /// <summary>
        /// Appends the specified string if the <paramref name="condition"/> is true
        /// </summary>
        /// <param name="condition">Indicates whether we append a newline to the end of this StringBuilder</param>
        public static StringBuilder Append(this StringBuilder builder, string value, int repeatCount = 1)
        {
            for (int i = 0; i < repeatCount; i++)
                builder.Append(value);
            return builder;
        }
    }
}
