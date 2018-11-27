using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Web.Mvc;
using System.Runtime.Caching;
using App.Client.Web.Models.Translation;
using App.Common.Helpers.InputOutput;
using App.Common.Helpers.Instance;
using Emit.LocalizationProvider.Concrete;
using Emit.LocalizationProvider.Configuration;

namespace App.Client.Web.Extensions
{
    public static class TranslationExtensions
    {
        private static readonly JsonLocalizationProvider Localization;

        private static object _lock = new object();

        private static MemoryCache _cache = new MemoryCache("TranslationCache");

        /// <summary>
        /// CTOR
        /// </summary>
        static TranslationExtensions()
        {
            Localization = new JsonLocalizationProvider();    
        }

        /// <summary>
        /// Return all available languages for translation
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAvailableLanguages()
        {
            return Localization.GetAvailableLanguages();
        }

        /// <summary>
        /// Return all available languages for translation
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAvailableLanguages(this HtmlHelper helper)
        {
            return Localization.GetAvailableLanguages();
        }

        /// <summary>
        /// Translation for views
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="key"></param>
        /// <param name="currentCultureInfo"></param>
        /// <param name="defaultCultureInfo"></param>
        /// <returns></returns>
        public static MvcHtmlString Translate(this HtmlHelper helper, string key, CultureInfo currentCultureInfo = null, CultureInfo defaultCultureInfo = null)
        {
            var cultureInfo = currentCultureInfo ?? Thread.CurrentThread.CurrentCulture;
            defaultCultureInfo = defaultCultureInfo ?? Thread.CurrentThread.CurrentCulture;
            var translation = Localization.GetTranslation(key, cultureInfo, defaultCultureInfo);
            return new MvcHtmlString(translation);
        }

        /// <summary>
        /// Translation for views
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="key"></param>
        /// <param name="fallbackKey"></param>
        /// <param name="currentCultureInfo"></param>
        /// <param name="defaultCultureInfo"></param>
        /// <returns></returns>
        public static MvcHtmlString Translate(this HtmlHelper helper, string key, string fallbackKey, CultureInfo currentCultureInfo = null, CultureInfo defaultCultureInfo = null)
        {
            var cultureInfo = currentCultureInfo ?? Thread.CurrentThread.CurrentCulture;
            defaultCultureInfo = defaultCultureInfo ?? Thread.CurrentThread.CurrentCulture;
            var translation = Localization.GetTranslation(key, cultureInfo, defaultCultureInfo);
            if (translation.IsNullOrEmpty()) translation = Localization.GetTranslation(fallbackKey, cultureInfo, defaultCultureInfo);
            return new MvcHtmlString(translation);
        }

        /// <summary>
        /// Translation for views
        /// </summary>
        /// <param name="key"></param>
        /// <param name="currentCultureInfo"></param>
        /// <param name="defaultCultureInfo"></param>
        /// <returns></returns>
        public static MvcHtmlString Translate(this string key, CultureInfo currentCultureInfo = null, CultureInfo defaultCultureInfo = null)
        {
            var cultureInfo = currentCultureInfo ?? Thread.CurrentThread.CurrentCulture;
            defaultCultureInfo = defaultCultureInfo ?? Thread.CurrentThread.CurrentCulture;
            var translation = Localization.GetTranslation(key, cultureInfo, defaultCultureInfo);
            return new MvcHtmlString(translation);
        }

        /// <summary>
        /// Page JSON translation string  
        /// </summary>
        /// <param name="serverPath"></param>
        /// <param name="translationFileName"></param>
        /// <returns></returns>
        public static string LoadAppJsonPageByName(string serverPath, string translationFileName)
        {
            var languageWithExtension = "." + Thread.CurrentThread.CurrentCulture.CompareInfo.Name + "." + JsonLocalizationProviderConfiguration.Instance.Extension;
            var fileName = translationFileName + languageWithExtension;
            var pageJsonPath = Path.Combine(serverPath, fileName);

            try
            {
                string translationJson;
                lock (_lock)
                {
                    translationJson = _cache.Get(pageJsonPath) as string;
                    if (translationJson.IsNullOrEmpty())
                    {
                        translationJson = IoHelper.ReadFile(pageJsonPath);
                        _cache.Set(pageJsonPath, translationJson, new CacheItemPolicy {SlidingExpiration = TimeSpan.FromDays(1)});
                    }
                }

                return translationJson;
            }
            catch (IOException e) 
            {
                return "{}";
            }
        }

        /// <summary>
        /// Extract pages parameters for translation from url query pipeline 
        /// pattern: [Folder],[File];[Folder],[File] or ?q=general,ui-form;landing,login
        /// </summary>
        /// <returns></returns>
        public static List<TranslationPagePathModel> ExctractPagesFromUrlQuery(string query)
        {
            // prevent empty string 
            if (query.IsNullOrEmpty()) { return new List<TranslationPagePathModel>(); }
            
            var pages = new List<TranslationPagePathModel>();

            // split pairs
            var pagePairs = query.Split(';');

            for (int i = 0; i < pagePairs.Length; i++)
            {
                var pair = pagePairs[i];
                // split folder and file names
                var splitPair = pair.Split(',');

                // Add pages that have folder and file define 
                if (!splitPair.IsNullOrHasZeroElements() && splitPair.Length == 2)
                {
                    pages.Add(new TranslationPagePathModel
                    {
                        Folder = splitPair[0],
                        File = splitPair[1]
                    });
                }
            }

            return pages;
        }

    }
}