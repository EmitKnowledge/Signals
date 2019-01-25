using Signals.Aspects.Localization.Entries;
using System.Collections.Generic;
using System.Globalization;

namespace Signals.Aspects.Localization
{
    /// <summary>
    /// Localization provider
    /// </summary>
    public interface ILocalizationProvider
    {
        /// <summary>
        /// Gets translation for the given key, nullable collection and category, and language (or current culture language if not provided)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="culture"></param>
        /// <param name="collection"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        LocalizationEntry Get(string key, string collection = null, string category = null, CultureInfo culture = null);

        /// <summary>
        /// Returns json representnation of a bundle of localization entries
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="collection"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        string GetLocalizationBundle(CultureInfo culture = null, string collection = null, string category = null);

        /// <summary>
        /// Returns list of all localization entries in the given language (or current culture language if not provided)
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="collection"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        List<LocalizationEntry> GetAllForCulture(CultureInfo culture = null, string collection = null, string category = null);

        /// <summary>
        /// Returns list of all localization entries
        /// </summary>
        /// <returns></returns>
        List<LocalizationEntry> GetAll();

        /// <summary>
        /// Sets new translation for the provided key and language (or current culture language if not provided)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="collection"></param>
        /// <param name="category"></param>
        /// <param name="culture"></param>
        void Set(string key, string value, string collection, string category, CultureInfo culture = null);

        /// <summary>
        /// Sets new translations for the provided entries
        /// </summary>
        /// <param name="entries"></param>
        void SetAll(List<LocalizationEntry> entries);

        /// <summary>
        /// Updates all localization entries
        /// </summary>
        /// <param name="entries"></param>
        void UpdateAll(List<LocalizationEntry> entries);

        /// <summary>
        /// Returns localization collection
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        LocalizationCollection GetCollection(string collection, string category = null);

        /// <summary>
        /// Returns list of all localization collections
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        List<LocalizationCollection> GetAllCollections(string category = null);

        /// <summary>
        /// Creates or updates collection
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="categoryName"></param>
        void SetCollection(string collectionName, string categoryName);

        /// <summary>
        /// Creates or updates collection
        /// </summary>
        /// <param name="collecitonName"></param>
        /// <param name="categoryId"></param>
        void SetCollection(string collecitonName, int categoryId);

        /// <summary>
        /// Returns localization category
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        LocalizationCategory GetCategory(string category);

        /// <summary>
        /// Returns all localization categories
        /// </summary>
        /// <returns></returns>
        List<LocalizationCategory> GetAllCategories();

        /// <summary>
        /// Creates or updates category
        /// </summary>
        /// <param name="categoryName"></param>
        void SetCategory(string categoryName);

        /// <summary>
        /// Creates new language
        /// </summary>
        /// <param name="culturaInfoName"></param>
        void InsertLanguage(string culturaInfoName);

        /// <summary>
        /// Returns list of all available keys
        /// </summary>
        /// <returns></returns>
        List<LocalizationKey> GetAllKeys(CultureInfo culture = null);

        /// <summary>
        /// Returns list of all available languages
        /// </summary>
        /// <returns></returns>
        List<LocalizationLanguage> GetAllLanguages();
    }
}
