using Newtonsoft.Json;
using Signals.Aspects.Localization.Entries;
using Signals.Aspects.Localization.File.Configurations;
using Signals.Aspects.Localization.File.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Signals.Aspects.Localization.File.DataProviders
{
    /// <summary>
    /// Localizaiton provider from json files
    /// </summary>
    public class JsonDataProvider : ILocalizationDataProvider
    {
        private const string LocalizationSearchRegex = @"\S+\.\S+.";
        private const string LocalizationMatchingRegex = @"(?<FILENAME>\S+)\.(?<LOCALIZATION>\S+).";

        private JsonDataProviderConfiguration Configuration { get; }

        private List<LocalizationCategory> Categories { get; set; }
        private List<LocalizationCollection> Collections { get; set; }
        private List<LocalizationEntry> Entries { get; set; }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="configuration"></param>
        public JsonDataProvider(JsonDataProviderConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Loads the localization entries
        /// </summary>
        /// <returns></returns>
        public List<LocalizationEntry> LoadLocalizationEntries()
        {
            var result = new List<LocalizationEntry>();
            foreach (var source in GetLocalizationSourcesPaths())
            {
                var files = IoHelper.SearchFiles(source.Value, LocalizationSearchRegex + Configuration.FileExtension);

                foreach (var file in files)
                {
                    var fileName = IoHelper.GetFileNameFromPath(file);

                    var parentDirectory = Directory.GetParent(file).Name;

                    // Get the match from the filename
                    var match = fileName.GetMatch(LocalizationMatchingRegex + Configuration.FileExtension);

                    // Read the localization file content
                    var fileContent = IoHelper.ReadTextFile(file);

                    // Deserialize the json content to dictionary
                    var localizationDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(fileContent);

                    if (localizationDictionary == null)
                    {
                        var message = $@"FILE: {file} HAS INVALID DATA.";
                        throw new InvalidDataException(message);
                    }

                    // Get the localization collection
                    var localizationCollection = GetLocalizationCollectionByName(match.Groups["FILENAME"].Value, parentDirectory);

                    // Get the localization code
                    var localizationCode = FormatLocalizationCode(match.Groups["LOCALIZATION"].Value);

                    foreach (var localizationPair in localizationDictionary)
                    {
                        result.Add(new LocalizationEntry
                        {
                            Value = localizationPair.Value,
                            LocalizationCollection = localizationCollection,
                            LocalizationKey = new LocalizationKey { Name = localizationPair.Key },
                            LocalizationLanguage = new LocalizationLanguage { Value = localizationCode }
                        });
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Loads the localization collections
        /// </summary>
        /// <returns></returns>
        public List<LocalizationCollection> LoadLocalizationCollections()
        {
            var result = new List<LocalizationCollection>();
            foreach (var source in GetLocalizationSourcesPaths())
            {
                var files = IoHelper.SearchFiles(source.Value, LocalizationSearchRegex + Configuration.FileExtension);
                foreach (var file in files)
                {
                    // Get the filename from the file
                    var fileName = IoHelper.GetFileNameFromPath(file);

                    // Get the match from the filename
                    var match = fileName.GetMatch(LocalizationMatchingRegex + Configuration.FileExtension);

                    var category = GetLocalizationCategoryByName(IoHelper.GetFileFolder(file));
                    result.Add(new LocalizationCollection
                    {
                        Name = match.Groups["FILENAME"].Value,
                        LocalizationCategory = category
                    });
                }
            }

            return result;
        }

        /// <summary>
        /// Loads the localization categories
        /// </summary>
        /// <returns></returns>
        public List<LocalizationCategory> LoadLocalizationCategories()
        {
            var result = new List<LocalizationCategory>();
            foreach (var source in GetLocalizationSourcesPaths())
            {
                var files = IoHelper.SearchFiles(source.Value, LocalizationSearchRegex + Configuration.FileExtension);
                foreach (var file in files)
                {
                    var folder = IoHelper.GetFileFolder(file);
                    if (result.All(x => x.Name != folder))
                        result.Add(new LocalizationCategory
                        {
                            Name = folder
                        });
                }
            }

            return result;
        }

        /// <summary>
        /// Loads the localizaiton keys
        /// </summary>
        /// <returns></returns>
        public List<LocalizationKey> LoadLocalizationKeys()
        {
            if (Entries == null)
                Entries = LoadLocalizationEntries();

            return Entries.GroupBy(x => x.LocalizationKey).Select(x => x.FirstOrDefault()?.LocalizationKey).ToList();
        }

        /// <summary>
        /// Loads the localization languages
        /// </summary>
        /// <returns></returns>
        public List<LocalizationLanguage> LoadLocalizationLanguages()
        {
            if (Entries == null)
                Entries = LoadLocalizationEntries();

            return Entries.GroupBy(x => x.LocalizationLanguageId).Select(x => x.FirstOrDefault()?.LocalizationLanguage).ToList();
        }

        /// <summary>
        /// Inserts new localization key
        /// </summary>
        /// <param name="localizationKey"></param>
        public void InsertLocalizationKey(LocalizationKey localizationKey)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Inserts new localization language
        /// </summary>
        /// <param name="localizationLanguage"></param>
        public void InsertLocalizationLanguage(LocalizationLanguage localizationLanguage)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Inserts new localization collection
        /// </summary>
        /// <param name="localizationCollection"></param>
        public void InsertOrUpdateLocalizationCollection(LocalizationCollection localizationCollection)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Inserts new localization entry
        /// </summary>
        /// <param name="entry"></param>
        public void InsertLocalizationEntry(LocalizationEntry entry)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Inserts new localization category
        /// </summary>
        /// <param name="localizationCategory"></param>
        public void InsertOrUpdateLocalizationCategory(LocalizationCategory localizationCategory)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Updates an existing localizaiton entry
        /// </summary>
        /// <param name="entry"></param>
        public void UpdateLocalizationEntry(LocalizationEntry entry)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Inserts or updates list of localization entries
        /// </summary>
        /// <param name="entries"></param>
        public void InsertOrUpdateLocalizationEnties(List<LocalizationEntry> entries)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Updates all localization entries
        /// </summary>
        /// <param name="entries"></param>
        public void UpdateAll(List<LocalizationEntry> entries)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Gets localization sources paths from configuration
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetLocalizationSourcesPaths()
        {
            var result = new Dictionary<string, string>();
            foreach (var source in Configuration.LocalizationSources)
            {
                result[source.Name] = string.IsNullOrEmpty(Configuration.DirectoryPath)
                    ? IoHelper.CombinePaths(ReflectionHelper.GetCurrentAssemblyRunPath(), source.SourcePath ?? string.Empty)
                    : IoHelper.CombinePaths(Configuration.DirectoryPath, source.SourcePath);
            }

            return result;
        }

        /// <summary>
        /// Gets the localization category by its name
        /// </summary>
        /// <param name="localizationCategoryName"></param>
        /// <returns></returns>
        private LocalizationCategory GetLocalizationCategoryByName(string localizationCategoryName)
        {
            if (Categories == null)
                Categories = LoadLocalizationCategories();

            return Categories.FirstOrDefault(x => x.Name == localizationCategoryName);
        }

        /// <summary>
        /// Gets the localization collection by its name
        /// </summary>
        /// <param name="localizationCollectionName"></param>
        /// <param name="localizationCategoryName"></param>
        /// <returns></returns>
        private LocalizationCollection GetLocalizationCollectionByName(string localizationCollectionName, string localizationCategoryName = null)
        {
            if (Collections == null)
                Collections = LoadLocalizationCollections();

            return Collections
                .FirstOrDefault(x => x.Name == localizationCollectionName &&
                                     (localizationCategoryName == null || x.LocalizationCategory.Name == localizationCategoryName));
        }

        /// <summary>
        /// Returns universal localization code
        /// </summary>
        /// <param name="localizationCode"></param>
        /// <returns></returns>
        protected static string FormatLocalizationCode(string localizationCode)
            => localizationCode.Split('-')[0].ToLower();
    }
}