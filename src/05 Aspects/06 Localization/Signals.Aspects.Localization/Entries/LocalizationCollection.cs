namespace Signals.Aspects.Localization.Entries
{
    /// <summary>
    /// Colieciton of localization entries
    /// </summary>
    public class LocalizationCollection
    {
        /// <summary>
        /// Default CTOR
        /// </summary>
        public LocalizationCollection() { }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="localizationCategoryId"></param>
        /// <param name="localizationCategory"></param>
        public LocalizationCollection(int id, string name, int localizationCategoryId, LocalizationCategory localizationCategory)
        {
            Id = id;
            Name = name;
            LocalizationCategoryId = localizationCategoryId;
            LocalizationCategory = localizationCategory;
        }

        /// <summary>
        /// Represents the localization collection Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Represents the localization collection name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Represents the localization collection category Id
        /// </summary>
        public int LocalizationCategoryId { get; set; }

        /// <summary>
        /// Represents the localization collection category
        /// </summary>
        public LocalizationCategory LocalizationCategory { get; set; }
    }
}
