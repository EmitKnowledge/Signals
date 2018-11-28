using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Signals.Aspects.Configuration.File
{
    /// <summary>
    /// File configuraiton provider
    /// </summary>
    public class FileConfigurationProvider : IConfigurationProvider
    {
        /// <summary>
        /// Path to the configuration
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Configuration filename
        /// </summary>
        public string File { get; set; }

        /// <summary>
        /// Indicates if the configuration should be reloaded on property access
        /// </summary>
        public bool ReloadOnAccess { get; set; }

        /// <summary>
        /// CTOR
        /// </summary>
        public FileConfigurationProvider()
        {
            Path = Environment.CurrentDirectory;
            File = "configuration.json";
        }

        /// <summary>
        /// Loads the configuration from mssql database
        /// </summary>
        public BaseConfiguration<T> Load<T>(string key) where T : BaseConfiguration<T>, new()
        {
            // Check if the key has been set
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("The configuration key must not be null or empty.");
            }

	        var fullPath = System.IO.Path.Combine(Path, File);
	        if (!System.IO.File.Exists(fullPath))
	        {
		        throw new ArgumentException("The configuration file does not exist.");
	        }

			// load the config file
	        var content = System.IO.File.ReadAllText(fullPath, Encoding.UTF8);
	        
	        // Deserialize the json config
	        var instance = JsonConvert.DeserializeObject<T>(content);
	        
	        // Validate if the configuration meets the validation rules
	        var valContext = new ValidationContext(instance);
	        var valResults = new List<ValidationResult>();
	        if (Validator.TryValidateObject(instance, valContext, valResults, true)) return instance;

	        throw new Exception(string.Join(Environment.NewLine, valResults.Select(x => x.ErrorMessage)));
        }

        /// <summary>
        /// Reloads the configuration
        /// </summary>
        public BaseConfiguration<T> Reload<T>(string key) where T : BaseConfiguration<T>, new()
        {
	        return ReloadOnAccess ? Load<T>(key) : null;
        }
    }
}
