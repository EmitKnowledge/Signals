namespace Signals.Aspects.Localization.Entries
{
    /// <summary>
    /// Category for localization entries
    /// </summary>
    public class LocalizationCategory
    {
        /// <summary>
        /// Default CTOR
        /// </summary>
        public LocalizationCategory() { }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        public LocalizationCategory(int id, string name)
        {
            Id = id;
            Name = name;
        }

        /// <summary>
        /// Represents the localizaiton category Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Represents the localization category name
        /// </summary>
        public string Name { get; set; }
    }
}
