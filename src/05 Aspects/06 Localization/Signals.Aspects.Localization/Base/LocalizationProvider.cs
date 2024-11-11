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
        /// Refreshes the localization entries cache
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
            => LocalizationEntries.FirstOrDefault(x => string.Equals(x.LocalizationKey?.Name, key, StringComparison.CurrentCultureIgnoreCase) &&
                                                       string.Equals(x.LocalizationLanguage?.Name, GetLanguageNameFromCulture(culture), StringComparison.CurrentCultureIgnoreCase) &&
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
            => SerializeLocalizationBundle(GetAllForCulture(culture, collection, category));

        /// <summary>
        /// Returns list of all localization entries
        /// </summary>
        /// <param name="culture"></param>
        /// <param name="collection"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        public List<LocalizationEntry> GetAllForCulture(CultureInfo culture = null, string collection = null, string category = null)
        {
            return LocalizationEntries
                .FindAll(x =>
                    string.Equals(x.LocalizationLanguage?.Name, GetLanguageNameFromCulture(culture), StringComparison.CurrentCultureIgnoreCase) &&
                    (string.IsNullOrEmpty(collection) || string.Equals(x.LocalizationCollection?.Name, collection, StringComparison.CurrentCultureIgnoreCase)) &&
                    (string.IsNullOrEmpty(category) || string.Equals(x.LocalizationCollection?.LocalizationCategory?.Name, category, StringComparison.CurrentCultureIgnoreCase)));
        }

        /// <summary>
        /// Returns list of all localization entries
        /// </summary>
        /// <returns></returns>
        public List<LocalizationEntry> GetAll()
        {
            return LocalizationEntries.ToList();
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
                var localizationKey = LocalizationKeys.FirstOrDefault(x => x.Name.ToLower() == key.ToLower());
                if (localizationKey == null)
                {
                    localizationKey = new LocalizationKey { Name = key };
                    Provider.InsertLocalizationKey(localizationKey);
                    LocalizationKeys = Provider.LoadLocalizationKeys();
                    localizationKey = LocalizationKeys.FirstOrDefault(x => x.Name.ToLower() == key.ToLower());
                }
                if (localizationKey == null) return;

                // Get the localization language. If it does not exist, create it first
                var langName = GetLanguageNameFromCulture(culture).ToLower();
                var langDisplayName = GetLanguageDisplayNameFromCulture(culture);
                var localizationLanguage = LocalizationLanguages.FirstOrDefault(x => x.Name.ToLower() == GetLanguageNameFromCulture(culture).ToLower());
                if (localizationLanguage == null)
                {
                    localizationLanguage = new LocalizationLanguage
                    {
                        Name = langName,
                        Value = langDisplayName
                    };
                    Provider.InsertLocalizationLanguage(localizationLanguage);
                    LocalizationLanguages = Provider.LoadLocalizationLanguages();
                    localizationLanguage = LocalizationLanguages.FirstOrDefault(x => x.Name.ToLower() == GetLanguageNameFromCulture(culture).ToLower());
                }
                if (localizationLanguage == null) return;

                // Get the localization collection. If it does not exist, create it first
                var localizationCategory = LocalizationCategories.FirstOrDefault(x => x.Name.ToLower() == category.ToLower());
                if (localizationCategory == null)
                {
                    localizationCategory = new LocalizationCategory
                    {
                        Name = category
                    };
                    Provider.InsertOrUpdateLocalizationCategory(localizationCategory);
                    LocalizationCategories = Provider.LoadLocalizationCategories();
                    localizationCategory = LocalizationCategories.FirstOrDefault(x => x.Name.ToLower() == category.ToLower());
                }
                if (localizationCategory == null) return;

                // Get the localization collection. If it does not exist, create it first
                var localizationCollection = LocalizationCollections.FirstOrDefault(x =>
                    x.Name.ToLower() == collection.ToLower() &&
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
                        x.Name.ToLower() == collection.ToLower() &&
                        x.LocalizationCategoryId == localizationCategory.Id);
                }
                if (localizationCollection == null) return;
                localizationCollection.LocalizationCategory = localizationCategory;
                localizationCollection.LocalizationCategoryId = localizationCategory.Id;

                // Don't check the collection since only one combination of key and language is allowed across collections
                var entry = LocalizationEntries.FirstOrDefault(x => x.LocalizationKeyId == localizationKey.Id &&
                                                                x.LocalizationLanguageId == localizationLanguage.Id);

                if (entry == null)
                {
                    // Create
                    entry = new LocalizationEntry
                    {
                        Value = value,
                        LocalizationCollectionId = localizationCollection.Id,
                        LocalizationKeyId = localizationKey.Id,
                        LocalizationLanguageId = localizationLanguage.Id,
                        CreatedOn = DateTime.UtcNow,
                        UpdatedOn = DateTime.UtcNow
                    };
                    Provider.InsertLocalizationEntry(entry);
                }
                else
                {
                    entry.Value = value;
					entry.LocalizationCollectionId = localizationCollection.Id;
					entry.LocalizationKeyId = localizationKey.Id;
					entry.LocalizationLanguageId = localizationLanguage.Id;
					Provider.UpdateLocalizationEntry(entry);
                }

				LocalizationEntries = Provider.LoadLocalizationEntries();
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
        /// Updates all localization entries
        /// </summary>
        /// <param name="entries"></param>
        public void UpdateAll(List<LocalizationEntry> entries)
        {
            lock (Lock)
            {
                Provider.UpdateAll(entries);
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
                x.Name.ToLower() == collection.ToLower() &&
                (string.IsNullOrEmpty(category) || x.LocalizationCategory?.Name.ToLower() == category.ToLower()));

        /// <summary>
        /// Returns list of all localization collections
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public List<LocalizationCollection> GetAllCollections(string category = null)
            => LocalizationCollections.FindAll(x =>
                string.IsNullOrEmpty(category) || x.LocalizationCategory?.Name.ToLower() == category.ToLower());

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
                var localizationCategory = LocalizationCategories.FirstOrDefault(x => x.Name.ToLower() == categoryName.ToLower());
                if (localizationCategory == null)
                {
                    localizationCategory = new LocalizationCategory
                    {
                        Name = categoryName
                    };
                    Provider.InsertOrUpdateLocalizationCategory(localizationCategory);
                    LocalizationCategories = Provider.LoadLocalizationCategories();
                    localizationCategory = LocalizationCategories.FirstOrDefault(x => x.Name.ToLower() == categoryName.ToLower());
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
        /// Creates new language
        /// </summary>
        /// <param name="culturaInfoName"></param>
        public void InsertLanguage(string culturaInfoName)
        {
            lock (Lock)
            {
                if (LocalizationLanguages.Any(x => x.Name.ToLower() == culturaInfoName.ToLower()))
                    return;

                if (CultureInfo.GetCultures(CultureTypes.AllCultures).All(x => x.Name.ToLower() != culturaInfoName.ToLower()))
                    return;

                // Insert the new language
                var cultureInfo = new CultureInfo(culturaInfoName);
                Provider.InsertLocalizationLanguage(new LocalizationLanguage
                {
                    Name = culturaInfoName,
                    Value = cultureInfo.DisplayName
                });

                LocalizationLanguages = Provider.LoadLocalizationLanguages();
                var newLang = LocalizationLanguages.FirstOrDefault(x => x.Name.ToLower() == culturaInfoName.ToLower());

                if (newLang == null)
                    return;

                // Create localization entry for the new language for each existing key
                var uniqueLocalizationEntries = (from entry in LocalizationEntries
                                                 group entry by entry.LocalizationKeyId into gr
                                                 select new
                                                 {
                                                     gr.Key,
                                                     gr.First().LocalizationCollectionId
                                                 }).ToList();

                var newLocalizationEntries = uniqueLocalizationEntries
                    .Select(x => new LocalizationEntry
                    {
                        LocalizationCollectionId = x.LocalizationCollectionId,
                        LocalizationLanguageId = newLang.Id,
                        LocalizationKeyId = x.Key,
                        CreatedOn = DateTime.UtcNow,
                        UpdatedOn= DateTime.UtcNow,
                        Value = string.Empty
                    })
                    .ToList();
                SetAll(newLocalizationEntries);
            }
        }

        /// <summary>
        /// Creates new localization key
        /// </summary>
        /// <param name="category"></param>
        /// <param name="collection"></param>
        /// <param name="key"></param>
        public void InsertKey(string category, string collection, string key)
        {
            lock (Lock)
            {
                foreach (var language in GetAllLanguages())
                {
                    Set(key.Replace(" ", "_"), string.Empty, collection, category, new CultureInfo(language.Name)); 
                }
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

        /// <summary>
        /// Returns all localization entries rendered as grid
        /// </summary>
        /// <returns></returns>
        public TranslationsGrid RenderTranslationsGrid()
        {
            var groupedEntries = (from entry in GetAll()
                                  group entry by entry.LocalizationKeyId into gr
                                  select new
                                  {
                                      Id = gr.Key,
                                      gr.First().LocalizationKey?.Name,
                                      Translations = gr.OrderBy(x => x.LocalizationLanguageId).Select(x => new GridTranslation
                                      {
                                          Id = x.Id,
                                          Name = x.LocalizationLanguage.Value,
                                          Translation = x.Value
                                      }).ToList(),
                                      Collection = gr.First().LocalizationCollection
                                  }).ToList();

            var groupedCollections = (from entry in groupedEntries
                                      group entry by entry.Collection.Id into gr
                                      select new
                                      {
                                          Id = gr.Key,
                                          gr.First().Collection.Name,
                                          Entries = gr.Select(x => new TranslationsGridEntry
                                          {
                                              Id = x.Id,
                                              Name = x.Name,
                                              Translations = x.Translations
                                          }).ToList(),
                                          Category = gr.First().Collection.LocalizationCategory
                                      }).ToList();

            var translations = (from collection in groupedCollections
                                group collection by collection.Category.Id into gr
                                select new TranslationsGridCategory
                                {
                                    Id = gr.Key,
                                    Name = gr.First().Category.Name,
                                    Collections = gr.Select(x => new TranslationGridCollection
                                    {
                                        Id = x.Id,
                                        Name = x.Name,
                                        Entries = x.Entries
                                    }).ToList()
                                }).ToList();

            return new TranslationsGrid
            {
                Translations = translations,
                Languages = GetAllLanguages().OrderBy(x => x.Id).ToList()
            };
        }

        private string GetLanguageDisplayNameFromCulture(CultureInfo culture)
            => culture?.DisplayName ?? CultureInfo.CurrentCulture.DisplayName;

        private string GetLanguageNameFromCulture(CultureInfo culture)
            => culture?.Name.ToLower() ?? CultureInfo.CurrentCulture.Name.ToLower();

        private string SerializeLocalizationBundle(List<LocalizationEntry> entries)
            => JsonConvert.SerializeObject(entries.ToDictionary(x => x.LocalizationKey.Name, x => x.Value));
    }
}