using System.Collections.Generic;

namespace Signals.Aspects.Localization.Entries
{
    /// <summary>
    /// 
    /// </summary>
    public class TranslationsGrid
    {
        /// <summary>
        /// 
        /// </summary>
        public List<TranslationsGridCategory> Translations { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<LocalizationLanguage> Languages { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class TranslationsGridCategory
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<TranslationGridCollection> Collections { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class TranslationGridCollection
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<TranslationsGridEntry> Entries { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class TranslationsGridEntry
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<GridTranslation> Translations { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class GridTranslation
    {
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Translation { get; set; }
    }
}
