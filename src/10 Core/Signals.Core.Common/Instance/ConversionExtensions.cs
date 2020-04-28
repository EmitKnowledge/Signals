using System;
using System.Globalization;
using System.Linq;

namespace Signals.Core.Common.Instance
{
    /// <summary>
    /// Extensions for conversion
    /// </summary>
    public static class ConversionExtensions
    {
        /// <summary>
        /// Convert string to bool
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ToBool(this string value)
        {
            bool.TryParse(value, out var outVal);
            return outVal;
        }

        /// <summary>
        /// Convert string to int
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int ToInt(this string value)
        {
            int.TryParse(value, out var outVal);
            return outVal;
        }

        /// <summary>
        /// Convert string to long
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long ToLong(this string value)
        {
            long.TryParse(value, out var outVal);
            return outVal;
        }

        /// <summary>
        /// Remove trailing zeros from the decimal
        /// </summary>
        /// <param name="value"></param>
        /// <param name="isCurrency"></param>
        /// <returns></returns>
        public static decimal RemoveTrailingZeros(this decimal value, bool isCurrency = false)
        {
            var output = value.ToString("G29");
            if (isCurrency)
            {
                var valueArgs = output.Split('.');
                if (valueArgs.Length == 2)
                {
                    if (valueArgs[1].Length == 1)
                    {
                        output = value.ToString("F");
                    }
                }
            }

            return Convert.ToDecimal(output);
        }

        /// <summary>
        /// Round a decimal value to the upper value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="decimals"></param>
        /// <param name="isCurrency"></param>
        /// <returns></returns>
        public static decimal RountToUpper(this decimal value, int decimals, bool isCurrency = false)
        {
            value = RemoveTrailingZeros(value, isCurrency);
            var valueAsString = value.ToString(CultureInfo.InvariantCulture);
            var valueArgs = valueAsString.Split('.').ToList();
            const int decimalPart = 1;

            if (valueArgs.Count == 2 &&
                valueArgs[decimalPart].Length < decimals)
            {
                while (valueArgs[decimalPart].Length < decimals)
                {
                    valueAsString = string.Concat(valueAsString, @"0");
                    valueArgs[decimalPart] = string.Concat(valueArgs[decimalPart], @"0");
                }
                return Convert.ToDecimal(valueAsString);
            }

            var multiplier = Math.Pow(10, decimals);
            var result = Math.Ceiling(Convert.ToDouble(value) * multiplier) / multiplier;
            return Convert.ToDecimal(result);
        }
    }
}