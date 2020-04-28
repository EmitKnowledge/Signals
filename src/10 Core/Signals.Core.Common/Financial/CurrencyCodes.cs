using Signals.Core.Common.Instance;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Signals.Core.Common.Financial
{
    /// <summary>
    /// Helper for managing currency codes
    /// </summary>
    public static class CurrencyCodes
    {
        private static readonly Dictionary<string, string> SymbolsByCode;

        private static readonly Dictionary<string, string> EnglishNameByCode;

        /// <summary>
        /// Format the the provided value with the provided currency
        /// Decide if the currency code should be placed left or right of the value.
        /// Ex. $ 50.00 or 50.00 MKD
        /// </summary>
        /// <param name="code"></param>
        /// <param name="value"></param>
        /// <param name="isCurrency"></param>
        /// <returns></returns>
        public static string Format(string code, decimal value, bool isCurrency = true)
        {
            var symbol = GetSymbol(code);
            var number = value.RemoveTrailingZeros(isCurrency);

            if (symbol?.Length > 1)
            {
                return $"{number} {symbol}";
            }

            return $"{symbol}{number}";
        }

        /// <summary>
        /// Return the currency symbol for the provided code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string GetSymbol(string code)
        {
            if (code.IsNullOrEmpty()) return code;
            if (!SymbolsByCode.ContainsKey(code)) return code;
            return SymbolsByCode[code];
        }

        /// <summary>
        /// Return the currency name for the provided code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string GetName(string code)
        {
            if (code.IsNullOrEmpty()) return code;
            if (!EnglishNameByCode.ContainsKey(code)) return code;
            return EnglishNameByCode[code];
        }

        /// <summary>
        /// CTOR
        /// </summary>
        static CurrencyCodes()
        {
            SymbolsByCode = new Dictionary<string, string>();
            EnglishNameByCode = new Dictionary<string, string>();

            var regions = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                                     .Select(x => new RegionInfo(x.LCID));

            foreach (var region in regions)
            {
                if (!SymbolsByCode.ContainsKey(region.ISOCurrencySymbol))
                {
                    SymbolsByCode.Add(region.ISOCurrencySymbol, region.CurrencySymbol);
                }
                if (!EnglishNameByCode.ContainsKey(region.ISOCurrencySymbol))
                {
                    EnglishNameByCode.Add(region.ISOCurrencySymbol, region.CurrencyEnglishName);
                }
            }
        }
    }
}