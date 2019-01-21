using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Newtonsoft.Json;
using Signals.Aspects.Localization.Entries;

namespace Signals.Aspects.Localization.Base
{
    /// <summary>
    /// Localization provider
    /// </summary>
    public class LocalizationProvider : ILocalizationProvider
    {
        /// <summary>
        /// Localizaiton data provider
        /// </summary>
        public ILocalizationDataProvider Provider { get; }

        /// <summary>
        /// Localization entries
        /// </summary>
        private static List<LocalizationEntry> LocalizationEntries { get; set; }

        /// <summary>
        /// Localization collections
        /// </summary>
        private static List<LocalizationCollection> LocalizationCollections { get; set; }

        /// <summary>
        /// Localization categories
        /// </summary>
        private static List<LocalizationCategory> LocalizationCategories { get; set; }

        /// <summary>
        /// Localization keys
        /// </summary>
        private static List<LocalizationKey> LocalizationKeys { get; set; }

        /// <summary>
        /// Localization languages
        /// </summary>
        private static List<LocalizationLanguage> LocalizationLanguages { get; set; }

        /// <summary>
        /// Thread lock
        /// </summary>
        private static readonly object Lock = new object();

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="provider"></param>
        public LocalizationProvider(ILocalizationDataProvider provider)
        {
            Provider = provider;
            Reload();
        }

        /// <summary>
        /// Loads the localization entries from the data provider
        /// </summary>
        public void Load()
        {
            LocalizationEntries = LocalizationEntries ?? Provider.LoadLocalizationEntries();
            LocalizationCollections = LocalizationCollections ?? Provider.LoadLocalizationCollections();
            LocalizationCategories = LocalizationCategories ?? Provider.LoadLocalizationCategories();
            LocalizationKeys = LocalizationKeys ?? Provider.LoadLocalizationKeys();
            LocalizationLanguages = LocalizationLanguages ?? Provider.LoadLocalizationLanguages();
        }

        /// <summary>
        /// Reloads the localization entries from the data provider
        /// </summary>
        public void Reload()
        {
            LocalizationEntries = Provider.LoadLocalizationEntries();
            LocalizationCollections = Provider.LoadLocalizationCollections();
            LocalizationCategories = Provider.LoadLocalizationCategories();
            LocalizationKeys = Provider.LoadLocalizationKeys();
            LocalizationLanguages = Provider.LoadLocalizationLanguages();
        }

        /// <summary>
        /// Gets translation for the given key and language (or current culture language if not provided)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="collection"></param>
        /// <param name="category"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public LocalizationEntry Get(string key, string collection = null, string category = null, CultureInfo culture = null)
            => LocalizationEntries.FirstOrDefault(x => x.LocalizationKey?.Name == key &&
                                                       x.LocalizationLanguage?.Value == GetLanguageCodeFromCulture(culture) &&
                                                       (collection == null || string.Equals(x.LocalizationCollection.Name, collection, StringComparison.CurrentCultureIgnoreCase)) &&
                                                       (category == null || string.Equals(x.LocalizationCollection.LocalizationCategory.Name, category, StringComparison.CurrentCultureIgnoreCase)));


        /// <summary>
        /// Returns json representnation of a bundle of localization entries
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="collection"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public string GetLocalizationBundle(CultureInfo culture = null, string collection = null, string category = null)
            => SerializeLocalizationBundle(GetAll(culture, collection, category));

        /// <summary>
        /// Returns list of all localization entries
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="collection"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public List<LocalizationEntry> GetAll(CultureInfo culture = null, string collection = null, string category = null)
        {
            return LocalizationEntries
                .FindAll(x =>
                    x.LocalizationLanguage?.Value == GetLanguageCodeFromCulture(culture) &&
                    (string.IsNullOrEmpty(collection) || x.LocalizationCollection?.Name == collection) &&
                    (string.IsNullOrEmpty(category) || x.LocalizationCollection?.LocalizationCategory?.Name == category));
        }

        /// <summary>
        /// Sets new translation for the provided key and language (or current culture language if not provided)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="collection"></param>
        /// <param name="category"></param>
        /// <param name="culture"></param>
        public void Set(string key, string value, string collection, string category, CultureInfo culture = null)
        {
            lock (Lock)
            {
                // Get the localization key Id. If it does not exist, create it first
                var localizationKey = LocalizationKeys.FirstOrDefault(x => x.Name == key);
                if (localizationKey == null)
                {
                    localizationKey = new LocalizationKey { Name = key };
                    Provider.InsertLocalizationKey(localizationKey);
                    LocalizationKeys = Provider.LoadLocalizationKeys();
                    localizationKey = LocalizationKeys.FirstOrDefault(x => x.Name == key);
                }
                if (localizationKey == null) return;

                // Get the localization language. If it does not exist, create it first
                var langCode = GetLanguageCodeFromCulture(culture);
                var langName = GetLanguageNameFromCulture(culture);
                var localizationLanguage = LocalizationLanguages.FirstOrDefault(x => x.Value == GetLanguageCodeFromCulture(culture));
                if (localizationLanguage == null)
                {
                    localizationLanguage = new LocalizationLanguage
                    {
                        Name = langName,
                        Value = langCode
                    };
                    Provider.InsertLocalizationLanguage(localizationLanguage);
                    LocalizationLanguages = Provider.LoadLocalizationLanguages();
                    localizationLanguage = LocalizationLanguages.FirstOrDefault(x => x.Value == GetLanguageCodeFromCulture(culture));
                }
                if (localizationLanguage == null) return;

                // Get the localization collection. If it does not exist, create it first
                var localizationCategory = LocalizationCategories.FirstOrDefault(x => x.Name == category);
                if (localizationCategory == null)
                {
                    localizationCategory = new LocalizationCategory
                    {
                        Name = category
                    };
                    Provider.InsertOrUpdateLocalizationCategory(localizationCategory);
                    LocalizationCategories = Provider.LoadLocalizationCategories();
                    localizationCategory = LocalizationCategories.FirstOrDefault(x => x.Name == category);
                }
                if (localizationCategory == null) return;

                // Get the localization collection. If it does not exist, create it first
                var localizationCollection = LocalizationCollections.FirstOrDefault(x =>
                    x.Name == collection &&
                    x.LocalizationCategoryId == localizationCategory.Id);

                if (localizationCollection == null)
                {
                    localizationCollection = new LocalizationCollection
                    {
                        Name = collection,
                        LocalizationCategoryId = localizationCategory.Id
                    };
                    Provider.InsertOrUpdateLocalizationCollection(localizationCollection);
                    LocalizationCollections = Provider.LoadLocalizationCollections();
                    localizationCollection = LocalizationCollections.FirstOrDefault(x =>
                        x.Name == collection &&
                        x.LocalizationCategoryId == localizationCategory.Id);
                }
                if (localizationCollection == null) return;
                localizationCollection.LocalizationCategory = localizationCategory;
                localizationCollection.LocalizationCategoryId = localizationCategory.Id;

                // Don't check the collection since only one combination of key and language is allowed across collections
                var entry = LocalizationEntries.FirstOrDefault(x => x.LocalizationKeyId == localizationKey.Id &&
                                                                x.LocalizationLanguageId == localizationLanguage.Id);

                if (entry != null)
                {
                    // Update
                    entry.Value = value;
                    entry.LocalizationCollectionId = localizationCollection.Id;
                    entry.LocalizationCollection = localizationCollection;
                    Provider.UpdateLocalizationEntry(entry);
                }
                else
                {
                    // Create
                    entry = new LocalizationEntry
                    {
                        Value = value,
                        LocalizationCollectionId = localizationCollection.Id,
                        LocalizationKeyId = localizationKey.Id,
                        LocalizationLanguageId = localizationLanguage.Id
                    };
                    Provider.InsertLocalizationEntry(entry);
                    LocalizationEntries = Provider.LoadLocalizationEntries();
                }
            }
        }

        /// <summary>
        /// Sets new translations for the provided entries
        /// </summary>
        /// <param name="entries"></param>
        public void SetAll(List<LocalizationEntry> entries)
        {
            lock (Lock)
            {
                Provider.InsertOrUpdateLocalizationEnties(entries);
                LocalizationEntries = Provider.LoadLocalizationEntries();
            }
        }

        /// <summary>
        /// Returns localization collection
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public LocalizationCollection GetCollection(string collection, string category = null)
            => LocalizationCollections.FirstOrDefault(x =>
                x.Name == collection &&
                (string.IsNullOrEmpty(category) || x.LocalizationCategory?.Name == category));

        /// <summary>
        /// Returns list of all localization collections
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public List<LocalizationCollection> GetAllCollections(string category = null)
            => LocalizationCollections.FindAll(x =>
                string.IsNullOrEmpty(category) || x.LocalizationCategory?.Name == category);

        /// <summary>
        /// Creates or updates collection
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="categoryName"></param>
        public void SetCollection(string collectionName, string categoryName)
        {
            lock (Lock)
            {
                // Get the localization collection. If it does not exist, create it first
                var localizationCategory = LocalizationCategories.FirstOrDefault(x => x.Name == categoryName);
                if (localizationCategory == null)
                {
                    localizationCategory = new LocalizationCategory
                    {
                        Name = categoryName
                    };
                    Provider.InsertOrUpdateLocalizationCategory(localizationCategory);
                    LocalizationCategories = Provider.LoadLocalizationCategories();
                    localizationCategory = LocalizationCategories.FirstOrDefault(x => x.Name == categoryName);
                }

                if (localizationCategory == null) return;

                var collection = LocalizationCollections.FirstOrDefault(x => x.Name.ToLower() == collectionName.ToLower())
                    ?? new LocalizationCollection
                    {
                        LocalizationCategoryId = localizationCategory.Id
                    };
                collection.Name = collectionName;
                Provider.InsertOrUpdateLocalizationCollection(collection);
                LocalizationCollections = Provider.LoadLocalizationCollections();
            }
        }

        /// <summary>
        /// Creates or updates collection
        /// </summary>
        /// <param name="collecitonName"></param>
        /// <param name="categoryId"></param>
        public void SetCollection(string collecitonName, int categoryId)
        {
            lock (Lock)
            {
                var localizationCategory = LocalizationCategories.FirstOrDefault(x => x.Id == categoryId);
                if (localizationCategory == null) return;

                Provider.InsertOrUpdateLocalizationCollection(new LocalizationCollection
                {
                    LocalizationCategoryId = localizationCategory.Id,
                    Name = collecitonName
                });
                LocalizationCollections = Provider.LoadLocalizationCollections();
            }
        }

        /// <summary>
        /// Returns localization category
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public LocalizationCategory GetCategory(string category)
        {
            return LocalizationCategories.FirstOrDefault(x => x.Name.ToLower() == category.ToLower());
        }

        /// <summary>
        /// Returns all localization categories
        /// </summary>
        /// <returns></returns>
        public List<LocalizationCategory> GetAllCategories()
        {
            return LocalizationCategories.ToList();
        }

        /// <summary>
        /// Creates or updates category
        /// </summary>
        /// <param name="categoryName"></param>
        public void SetCategory(string categoryName)
        {
            lock (Lock)
            {
                var localizationCategory = GetCategory(categoryName) ?? new LocalizationCategory();
                localizationCategory.Name = categoryName;

                Provider.InsertOrUpdateLocalizationCategory(localizationCategory);
                LocalizationCategories = Provider.LoadLocalizationCategories();
            }
        }

        /// <summary>
        /// Returns list of all available keys
        /// </summary>
        /// <returns></returns>
        public List<LocalizationKey> GetAllKeys(CultureInfo culture = null)
        {
            return LocalizationKeys.ToList();
        }

        /// <summary>
        /// Returns list of all available languages
        /// </summary>
        /// <returns></returns>
        public List<LocalizationLanguage> GetAllLanguages()
        {
            return LocalizationLanguages.ToList();
        }

        private string GetLanguageCodeFromCulture(CultureInfo culture)
            => culture?.TwoLetterISOLanguageName ?? CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

        private string GetLanguageNameFromCulture(CultureInfo culture)
            => culture?.DisplayName ?? CultureInfo.CurrentCulture.DisplayName;

        private string SerializeLocalizationBundle(List<LocalizationEntry> entries)
            => JsonConvert.SerializeObject(entries.ToDictionary(x => x.LocalizationKey.Name, x => x.Value));
    }
}