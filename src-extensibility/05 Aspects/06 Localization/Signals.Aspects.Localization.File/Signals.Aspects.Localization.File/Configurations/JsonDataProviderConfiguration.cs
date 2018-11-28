using System;
using System.Collections.Generic;

namespace Signals.Aspects.Localization.File.Configurations
{
    /// <summary>
    /// Configuration for json file provider
    /// </summary>
    public class JsonDataProviderConfiguration : ILocalizationConfiguration
    {
        /// <summary>
        /// The file directory where the localizations are stored
        /// </summary>
        public string DirectoryPath { get; set; }

        /// <summary>
        /// Represents the list of sources the localization entries are coming from
        /// </summary>
        public List<LocalizationSource> LocalizationSources { get; set; }

        /// <summary>
        /// Represents the file extensions the localization files have
        /// </summary>
        public string FileExtension { get; set; }

        /// <summary>
        /// CTOR
        /// </summary>
        public JsonDataProviderConfiguration()
        {
            DirectoryPath = AppDomain.CurrentDomain.BaseDirectory;
            FileExtension = "app";
            LocalizationSources = new List<LocalizationSource>();
        }
    }

    /// <summary>
    /// Localizaiton source
    /// </summary>
    public class LocalizationSource
    {
        /// <summary>
        /// Represents the source name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Represents the source path
        /// </summary>
        public string SourcePath { get; set; }

        /// <summary>
        /// Indicates whether the base directory should be used
        /// </summary>
        public bool UseBaseDirectory { get; set; }
    }
}
