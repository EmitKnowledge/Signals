using RazorLight;
using Signals.Aspects.Localization;
using Signals.Aspects.Localization.Entries;
using System.Globalization;
using System.Web;

namespace App.Common.Helpers.Localizaiton
{
    /// <summary>
    /// Extensions for localizaiton profider
    /// </summary>
    public static class LocalizationExtensions
    {
        /// <summary>
        /// Render razor template using model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entry"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static string RenderTemplate<T>(this LocalizationEntry entry, T model)
        {
            if (entry == null) return null;
            if (model == null) return entry.Value;

            var engine = new RazorLightEngineBuilder()
                 .UseMemoryCachingProvider()
                 .Build();

            var key = $"{entry?.LocalizationCollection?.LocalizationCategory?.Name}-{entry?.LocalizationCollection?.Name}-{entry?.LocalizationKey?.Name}";

            string result = engine
                .CompileRenderAsync(key, entry.Value, model)
                .GetAwaiter()
                .GetResult();

            result = HttpUtility.HtmlDecode(result);

            return result;
        }

        /// <summary>
        /// Translate key in all colections
        /// </summary>
        /// <param name="localizationProvider"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Translate(this ILocalizationProvider localizationProvider, string key, CultureInfo culture = null)
        {
            if (string.IsNullOrEmpty(key)) return null;

            var value = localizationProvider.Get(key, culture: culture)?.Value;
            return value ?? $"Oops! {key} is not translated!";
        }
    }
}