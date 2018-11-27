using Signals.Aspects.Localization.Entries;
using System.Collections.Generic;

namespace Signals.Aspects.Localization
{
    /// <summary>
    /// Localization data provider
    /// </summary>
    public interface ILocalizationDataProvider
    {
        /// <summary>
        /// Loads the localization entries
        /// </summary>
        /// <returns></returns>
        List<LocalizationEntry> LoadLocalizationEntries();

        /// <summary>
        /// Loads the localization collections
        /// </summary>
        /// <returns></returns>
        List<LocalizationCollection> LoadLocalizationCollections();

        /// <summary>
        /// Loads the localization categories
        /// </summary>
        /// <returns></returns>
        List<LocalizationCategory> LoadLocalizationCategories();

        /// <summary>
        /// Loads the localizaiton keys
        /// </summary>
        /// <returns></returns>
        List<LocalizationKey> LoadLocalizationKeys();

        /// <summary>
        /// Loads the localization languages
        /// </summary>
        /// <returns></returns>
        List<LocalizationLanguage> LoadLocalizationLanguages();

        /// <summary>
        /// Inserts new localization key
        /// </summary>
        /// <param name="localizationKey"></param>
        void InsertLocalizationKey(LocalizationKey localizationKey);

        /// <summary>
        /// Inserts new localization language
        /// </summary>
        /// <param name="localizationLanguage"></param>
        void InsertLocalizationLanguage(LocalizationLanguage localizationLanguage);

        /// <summary>
        /// Inserts new localization collection
        /// </summary>
        /// <param name="localizationCollection"></param>
        void InsertOrUpdateLocalizationCollection(LocalizationCollection localizationCollection);

        /// <summary>
        /// Inserts new localization entry
        /// </summary>
        /// <param name="entry"></param>
        void InsertLocalizationEntry(LocalizationEntry entry);

        /// <summary>
        /// Inserts new localization category
        /// </summary>
        /// <param name="localizationCategory"></param>
        void InsertOrUpdateLocalizationCategory(LocalizationCategory localizationCategory);

        /// <summary>
        /// Updates an existing localizaiton entry
        /// </summary>
        /// <param name="entry"></param>
        void UpdateLocalizationEntry(LocalizationEntry entry);

        /// <summary>
        /// Inserts or updates list of localization entries
        /// </summary>
        /// <param name="entries"></param>
        void InsertOrUpdateLocalizationEnties(List<LocalizationEntry> entries);
    }
}