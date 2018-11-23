using Signals.Core.Common.Instance;

namespace Signals.Core.Common.Financial
{
    public static class PercentageExtensions
    {
        /// <summary>
        /// Get @percent of @val
        /// </summary>
        /// <param name="val">100.0</param>
        /// <param name="fromValue"></param>
        /// <param name="deciamlPlaces"></param>
        /// <returns>20.0</returns>
        public static decimal GetPercent(decimal val, decimal fromValue, int deciamlPlaces = 4)
        {
            return decimal.Round((val * 100) / fromValue, deciamlPlaces).RemoveTrailingZeros();
        }

        /// <summary>
        /// Get @percent of @val
        /// </summary>
        /// <param name="val">100.0</param>
        /// <param name="percent">20.0</param>
        /// <returns>20.0</returns>
        public static double GetAmount(double val, double percent)
        {
            return val * percent / 100;
        }

        /// <summary>
        /// Get @percent of @val
        /// </summary>
        /// <param name="val">100.0</param>
        /// <param name="percent">20.0</param>
        /// <returns>20.0</returns>
        public static float GetAmount(float val, float percent)
        {
            return val * percent / 100;
        }

        /// <summary>
        /// Get @percent of @val
        /// </summary>
        /// <param name="val">100.0</param>
        /// <param name="percent">20.0</param>
        /// <param name="decimalPlaces"></param>
        /// <returns>20.0</returns>
        public static decimal GetAmount(decimal val, decimal percent, int decimalPlaces = 4)
        {
            return decimal.Round(val * percent / 100, decimalPlaces);
        }

        /// <summary>
        /// Get @percent of @val
        /// </summary>
        /// <param name="val">100</param>
        /// <param name="percent">20</param>
        /// <returns>20.0</returns>
        public static double GetAmount(int val, int percent)
        {
            return (double)val * percent / 100;
        }

        /// <summary>
        /// Substract @percent of @val
        /// </summary>
        /// <param name="val">100.0</param>
        /// <param name="percent">20.0</param>
        /// <returns>80.0</returns>
        public static double SubstractPercent(double val, double percent)
        {
            return val * (100 - percent) / 100;
        }

        /// <summary>
        /// Substract @percent of @val
        /// </summary>
        /// <param name="val">100.0</param>
        /// <param name="percent">20.0</param>
        /// <returns>80.0</returns>
        public static float SubstractPercent(float val, float percent)
        {
            return val * (100 - percent) / 100;
        }

        /// <summary>
        /// Substract @percent of @val
        /// </summary>
        /// <param name="val">100.0</param>
        /// <param name="percent">20.0</param>
        /// <returns>80.0</returns>
        public static decimal SubstractPercent(decimal val, decimal percent)
        {
            return val * (100 - percent) / 100;
        }

        /// <summary>
        /// Substract @percent of @val
        /// </summary>
        /// <param name="val">100</param>
        /// <param name="percent">20</param>
        /// <returns>80.0</returns>
        public static double SubstractPercent(int val, int percent)
        {
            return (double)val * (100 - percent) / 100;
        }

        /// <summary>
        /// Add percent to value if you add 20% to value of 100 you will get 120 as result
        /// </summary>
        /// <param name="val"></param>
        /// <param name="percent"></param>
        /// <param name="decimalPlaces"></param>
        /// <returns></returns>
        public static decimal AddPercent(decimal val, decimal percent, int decimalPlaces = 4)
        {
            return decimal.Round((val + (val * percent) / 100), decimalPlaces);
        }
    }
}