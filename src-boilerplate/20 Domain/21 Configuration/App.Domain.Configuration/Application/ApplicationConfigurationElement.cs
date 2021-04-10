using System.ComponentModel.DataAnnotations;

namespace App.Domain.Configuration.Application
{
    public sealed class ApplicationConfigurationElement
    {
        /// <summary>
        /// Name of the project to whom this configuration relates to
        /// </summary>
        [Required]
        public string ProjectName { get; set; }

        /// <summary>
        /// Name of the application to whom this configuration relates to
        /// </summary>
        [Required]
        public string ApplicationName { get; set; }

        /// <summary>
        /// Name of the environment to which this configuration applies
        /// </summary>
        public string Enviroment { get; set; }
    }
}