namespace Signals.Aspects.Localization.Database.Configurations
{
    /// <summary>
    /// Configuration for data provider for database
    /// </summary>
    public class DatabaseDataProviderConfiguration : ILocalizationConfiguration
    {
        /// <summary>
        /// Represents the database connection string
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Represents the name of the table storing the translations
        /// </summary>
        public string LocalizationEntryTableName { get; set; }

        /// <summary>
        /// Represents the name of the table storing the collections
        /// </summary>
        public string LocalizationCollectionTableName { get; set; }

        /// <summary>
        /// Represents the name of the table storing the categories
        /// </summary>
        public string LocalizationCategoryTableName { get; set; }

        /// <summary>
        /// Represents the name of the table storing the keys
        /// </summary>
        public string LocalizationKeyTableName { get; set; }

        /// <summary>
        /// Represents the name of the table storing the languages
        /// </summary>
        public string LocalizationLanguageTableName { get; set; }

        /// <summary>
        /// CTOR With ConnectionString
        /// </summary>
        /// <param name="connectionString"></param>
        public DatabaseDataProviderConfiguration(string connectionString)
        {
            ConnectionString = connectionString;
            LocalizationCategoryTableName = "LocalizationCategory";
            LocalizationCollectionTableName = "LocalizationCollection";
            LocalizationEntryTableName = "LocalizationEntry";
            LocalizationKeyTableName = "LocalizationKey";
            LocalizationLanguageTableName = "LocalizationLanguage";
        }

        /// <summary>
        /// CTOR
        /// </summary>
        public DatabaseDataProviderConfiguration()
        {
            LocalizationCategoryTableName = "LocalizationCategory";
            LocalizationCollectionTableName = "LocalizationCollection";
            LocalizationEntryTableName = "LocalizationEntry";
            LocalizationKeyTableName = "LocalizationKey";
            LocalizationLanguageTableName = "LocalizationLanguage";
        }
    }
}
