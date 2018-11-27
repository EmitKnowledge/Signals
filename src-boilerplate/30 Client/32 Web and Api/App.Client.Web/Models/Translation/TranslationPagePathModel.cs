namespace App.Client.Web.Models.Translation
{
    public class TranslationPagePathModel
    {
        /// <summary>
        /// Folder name where of the translation in resources destination
        /// </summary>
        public string Folder { get; set; }

        /// <summary>
        /// File name of the localization folder in resources destination
        /// </summary>
        public string File { get; set; }

        public TranslationPagePathModel()
        {
            Folder = "";
            File = "";
        }

    }
}