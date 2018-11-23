namespace Signals.Aspects.Localization.Entries
{
    public class LocalizationLanguage
    {
        /// <summary>
        /// Default CTOR
        /// </summary>
        public LocalizationLanguage() { }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public LocalizationLanguage(int id, string name, string value)
        {
            Id = id;
            Name = name;
            Value = value;
        }

        /// <summary>
        /// Represents the language ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Represents the language name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Represents the language two-letter ISO representation
        /// </summary>
        public string Value { get; set; }
    }
}
