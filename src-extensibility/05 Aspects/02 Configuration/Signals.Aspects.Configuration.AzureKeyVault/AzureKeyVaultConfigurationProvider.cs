using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Signals.Aspects.Configuration.AzureKeyVault
{
    /// <summary>
    /// Read-only configuration provider for Azure Key Vault JSON or individual property secrets.
    /// </summary>
    public class AzureKeyVaultConfigurationProvider : IConfigurationProvider
    {
        private readonly SecretClient _secretClient;

        /// <summary>
        /// Creates a provider using DefaultAzureCredential.
        /// </summary>
        public AzureKeyVaultConfigurationProvider(string vaultUri)
            : this(new Uri(vaultUri), new DefaultAzureCredential())
        {
        }

        /// <summary>
        /// Creates a provider using DefaultAzureCredential.
        /// </summary>
        public AzureKeyVaultConfigurationProvider(Uri vaultUri)
            : this(vaultUri, new DefaultAzureCredential())
        {
        }

        /// <summary>
        /// Creates a provider using the supplied token credential.
        /// </summary>
        public AzureKeyVaultConfigurationProvider(Uri vaultUri, TokenCredential credential)
            : this(new SecretClient(vaultUri, credential))
        {
        }

        /// <summary>
        /// Creates a provider using an existing Key Vault secret client.
        /// </summary>
        public AzureKeyVaultConfigurationProvider(SecretClient secretClient)
        {
            _secretClient = secretClient ?? throw new ArgumentNullException(nameof(secretClient));
        }

        /// <summary>
        /// Optional prefix applied to every resolved secret name.
        /// </summary>
        public string SecretPrefix { get; set; }

        /// <summary>
        /// When true, loads each writable property from a secret named SecretPrefix + property.Name.
        /// Settings objects contain JSON and are deserialized into the property's type.
        /// Defaults to false: reads the complete configuration JSON from one secret named SecretPrefix + configuration.Key.
        /// Example: Key = "SecretConfiguration" and SecretPrefix = "dev-" read secret "dev-SecretConfiguration"
        /// with value {"Smtp":{"Username":"mailer@example.com","Password":"example-password"}}.
        /// </summary>
        public bool UsePerPropertySecrets { get; set; }

        /// <summary>
        /// Indicates if the configuration should be reloaded whenever it's accessed.
        /// </summary>
        public bool ReloadOnAccess { get; set; }

        /// <summary>
        /// Loads configuration from one JSON secret or individual property secrets.
        /// </summary>
        public BaseConfiguration<T> Load<T>(string key) where T : BaseConfiguration<T>, new()
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("The configuration key must not be null or empty.");
            }

            T instance;
            if (UsePerPropertySecrets)
            {
                instance = new T();
                foreach (var property in GetConfigurationProperties(typeof(T)))
                {
                    var secretName = GetSecretName(property.Name);
                    var content = GetSecretValue(secretName);
                    if (content == null) continue;

                    try
                    {
                        if (property.PropertyType != typeof(string) && string.IsNullOrWhiteSpace(content))
                        {
                            throw new JsonSerializationException("Expected a JSON value.");
                        }

                        var value = property.PropertyType == typeof(string)
                            ? content
                            : JsonConvert.DeserializeObject(content, property.PropertyType);

                        property.SetValue(instance, value);
                    }
                    catch (JsonException)
                    {
                        // Json.NET errors can contain the secret value, so report only its name.
                        throw new InvalidOperationException($"Secret '{secretName}' could not be deserialized as '{property.PropertyType.FullName}'.");
                    }
                }
            }
            else
            {
                var secretName = GetSecretName(key);
                var content = GetSecretValue(secretName);
                try
                {
                    instance = content == null ? new T() : JsonConvert.DeserializeObject<T>(content);
                }
                catch (JsonException)
                {
                    // Do not retain the original exception, which can contain secret values.
                    throw new InvalidOperationException($"Secret '{secretName}' could not be deserialized as '{typeof(T).FullName}'.");
                }

                if (instance == null)
                {
                    throw new Exception($"The Azure Key Vault secret '{secretName}' could not be deserialized.");
                }
            }

            var valContext = new ValidationContext(instance);
            var valResults = new List<ValidationResult>();
            if (Validator.TryValidateObject(instance, valContext, valResults, true)) 
                return instance;

            throw new Exception(string.Join(Environment.NewLine, valResults.Select(x => x.ErrorMessage)));
        }

        /// <summary>
        /// Reloads the configuration.
        /// </summary>
        public BaseConfiguration<T> Reload<T>(string key) where T : BaseConfiguration<T>, new()
        {
            return Load<T>(key);
        }

        /// <summary>
        /// Updating configuration is not supported. Secrets are managed directly in Azure Key Vault.
        /// </summary>
        public void Update<T>(T configuration) where T : BaseConfiguration<T>, new()
        {
            throw new MissingMethodException("Cannot update configuration key vault");
        }

        private static IEnumerable<PropertyInfo> GetConfigurationProperties(Type type)
        {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(property => property.SetMethod?.IsPublic == true
                    && property.GetIndexParameters().Length == 0);
        }

        private string GetSecretName(string key)
        {
            return string.Concat(SecretPrefix, key);
        }

        private string GetSecretValue(string secretName)
        {
            try
            {
                return _secretClient.GetSecret(secretName).Value.Value;
            }
            catch (RequestFailedException ex) when (ex.Status == 404 && ex.ErrorCode == "SecretNotFound")
            {
                return null;
            }
        }
    }
}
