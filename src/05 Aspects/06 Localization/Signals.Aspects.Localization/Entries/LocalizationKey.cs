namespace Signals.Aspects.Localization.Entries
{
    /// <summary>
    /// Key for localizaiton entry
    /// </summary>
    public class LocalizationKey
    {
        /// <summary>
        /// Default CTOR
        /// </summary>
        public LocalizationKey() { }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        public LocalizationKey(int id, string name)
        {
            Id = id;
            Name = name;
        }

        /// <summary>
        /// Represents the key's ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Represents the key's name
        /// </summary>
        public string Name { get; set; }
    }
}
