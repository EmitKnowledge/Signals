using System;

namespace Signals.Aspects.Localization.Entries
{
    /// <summary>
    /// Localization entry
    /// </summary>
    public class LocalizationEntry
    {
        /// <summary>
        /// Represents the entry's ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Represents the entry's creation date
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Represents the entry's last update date
        /// </summary>
        public DateTime UpdatedOn { get; set; }

        /// <summary>
        /// Represents the entry's value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Represents the entry's language Id
        /// </summary>
        public int LocalizationLanguageId { get; set; }

        /// <summary>
        /// Represents the entry's language
        /// </summary>
        public LocalizationLanguage LocalizationLanguage { get; set; }

        /// <summary>
        /// Represents the entry's key Id
        /// </summary>
        public int LocalizationKeyId { get; set; }

        /// <summary>
        /// Represents the entry's key
        /// </summary>
        public LocalizationKey LocalizationKey { get; set; }

        /// <summary>
        /// Represents the entry's collection
        /// </summary>
        public int LocalizationCollectionId { get; set; }

        /// <summary>
        /// Represents the entry's collection
        /// </summary>
        public LocalizationCollection LocalizationCollection { get; set; }
    }
}
