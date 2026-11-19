using Azure;
using Azure.Security.KeyVault.Secrets;
using Signals.Aspects.Configuration.AzureKeyVault;
using Signals.Tests.Configuration.CustomConfigurations.AzureKeyVault;
using Signals.Tests.Configuration.Fakes;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Signals.Tests.Configuration
{
    public class AzureKeyVaultConfigurationTests
    {
        [Fact]
        public void Constructor_Null_Secret_Client_Should_Throw()
        {
            var exception = Assert.Throws<ArgumentNullException>(() =>
                new AzureKeyVaultConfigurationProvider((SecretClient)null));

            Assert.Equal("secretClient", exception.ParamName);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData(null, true)]
        [InlineData("", false)]
        [InlineData("", true)]
        public void Load_Missing_Key_Should_Throw_Without_Accessing_Key_Vault(string key, bool perProperty)
        {
            var client = new InMemorySecretClient();
            var provider = new AzureKeyVaultConfigurationProvider(client) { UsePerPropertySecrets = perProperty };

            var exception = Assert.Throws<ArgumentException>(() => provider.Load<SecretsConfiguration>(key));

            Assert.Contains("configuration key", exception.Message);
            Assert.Empty(client.RequestedSecrets);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("dev-")]
        public void Load_Whole_Configuration_Should_Deserialize_Json(string prefix)
        {
            var secretName = prefix + nameof(SecretsConfiguration);
            var client = new InMemorySecretClient(new Dictionary<string, string>
            {
                [secretName] = @"{
                    ""Smtp"": { ""Username"": ""mailer@example.com"", ""Password"": ""mock-password"" },
                    ""ApiKey"": ""mock-api-key"",
                    ""RetryCount"": 7,
                    ""TimeoutSeconds"": 2.5,
                    ""Enabled"": false
                }"
            });
            var provider = new AzureKeyVaultConfigurationProvider(client) { SecretPrefix = prefix };

            SecretsConfiguration.UseProvider(provider);
            var configuration = SecretsConfiguration.Instance;

            Assert.False(provider.UsePerPropertySecrets);
            Assert.NotNull(configuration.Smtp);
            Assert.Equal("mailer@example.com", configuration.Smtp.Username);
            Assert.Equal("mock-password", configuration.Smtp.Password);
            Assert.Equal("mock-api-key", configuration.ApiKey);
            Assert.Equal(7, configuration.RetryCount);
            Assert.Equal(2.5f, configuration.TimeoutSeconds);
            Assert.False(configuration.Enabled);
            Assert.Equal(new[] { secretName }, client.RequestedSecrets);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("dev-")]
        public void Load_Per_Property_Should_Deserialize_Objects_And_Scalar_Values(string prefix)
        {
            var client = new InMemorySecretClient(new Dictionary<string, string>
            {
                [prefix + "Smtp"] = @"{ ""Username"": ""mailer@example.com"", ""Password"": ""mock-password"" }",
                [prefix + "ApiKey"] = "plain-api-key+/=",
                [prefix + "RetryCount"] = "7",
                [prefix + "TimeoutSeconds"] = "2.5",
                [prefix + "Enabled"] = "false"
            });
            var provider = new AzureKeyVaultConfigurationProvider(client)
            {
                SecretPrefix = prefix,
                UsePerPropertySecrets = true
            };

            SecretsConfiguration.UseProvider(provider);
            var configuration = SecretsConfiguration.Instance;

            Assert.NotNull(configuration.Smtp);
            Assert.Equal("mailer@example.com", configuration.Smtp.Username);
            Assert.Equal("mock-password", configuration.Smtp.Password);
            Assert.Equal("plain-api-key+/=", configuration.ApiKey);
            Assert.Equal(7, configuration.RetryCount);
            Assert.Equal(2.5f, configuration.TimeoutSeconds);
            Assert.False(configuration.Enabled);
            Assert.Equal(5, client.RequestedSecrets.Count);
            Assert.Contains(prefix + "Smtp", client.RequestedSecrets);
            Assert.DoesNotContain(prefix + nameof(SmtpSecrets), client.RequestedSecrets);
            Assert.DoesNotContain(prefix + nameof(SecretsConfiguration), client.RequestedSecrets);
        }

        [Fact]
        public void Load_Whole_Configuration_Should_Use_Configuration_Key()
        {
            var client = new InMemorySecretClient(new Dictionary<string, string>
            {
                ["dev-CustomKey"] = @"{ ""ApiKey"": ""mock-api-key"" }"
            });
            var provider = new AzureKeyVaultConfigurationProvider(client) { SecretPrefix = "dev-" };

            CustomSecretsConfiguration.UseProvider(provider);

            Assert.Equal("mock-api-key", CustomSecretsConfiguration.Instance.ApiKey);
            Assert.Equal(new[] { "dev-CustomKey" }, client.RequestedSecrets);
        }

        [Fact]
        public void Load_Per_Property_Should_Ignore_Readonly_Static_And_Indexed_Properties()
        {
            var client = new InMemorySecretClient(new Dictionary<string, string>
            {
                ["dev-ApiKey"] = "mock-api-key",
                ["dev-ReadOnlyValue"] = "unused",
                ["dev-PrivateSetValue"] = "unused",
                ["dev-StaticValue"] = "unused",
                ["dev-Item"] = "unused"
            });
            var provider = new AzureKeyVaultConfigurationProvider(client)
            {
                SecretPrefix = "dev-",
                UsePerPropertySecrets = true
            };

            CustomSecretsConfiguration.UseProvider(provider);
            var configuration = CustomSecretsConfiguration.Instance;

            Assert.Equal("mock-api-key", configuration.ApiKey);
            Assert.Equal("read-only", configuration.ReadOnlyValue);
            Assert.Equal("private-set", configuration.PrivateSetValue);
            Assert.Equal(new[] { "dev-ApiKey" }, client.RequestedSecrets);
        }

        [Theory]
        [InlineData("{")]
        [InlineData("[]")]
        [InlineData("\"sensitive-config-value\"")]
        [InlineData("{\"RetryCount\":\"sensitive-config-value\"}")]
        [InlineData("{\"ApiKey\":\"mock-api-key\",\"Smtp\":\"sensitive-config-value\"}")]
        [InlineData("{\"ApiKey\":\"sensitive-config-value\"")]
        public void Load_Invalid_Whole_Configuration_Json_Should_Report_Name_Without_Secret_Value(string content)
        {
            var client = new InMemorySecretClient(new Dictionary<string, string>
            {
                ["dev-SecretsConfiguration"] = content
            });
            var provider = new AzureKeyVaultConfigurationProvider(client) { SecretPrefix = "dev-" };

            var exception = Assert.Throws<InvalidOperationException>(() => SecretsConfiguration.UseProvider(provider));

            Assert.Contains("dev-SecretsConfiguration", exception.Message);
            Assert.Contains(typeof(SecretsConfiguration).FullName ?? string.Empty, exception.Message);
            Assert.DoesNotContain("sensitive-config-value", exception.ToString());
            Assert.DoesNotContain("mock-api-key", exception.ToString());
            Assert.Null(exception.InnerException);
            Assert.Equal(new[] { "dev-SecretsConfiguration" }, client.RequestedSecrets);
        }

        [Theory]
        [InlineData("null")]
        [InlineData("")]
        [InlineData("   ")]
        public void Load_Whole_Configuration_Without_An_Object_Should_Report_Secret_Name(string content)
        {
            var client = new InMemorySecretClient(new Dictionary<string, string>
            {
                ["dev-SecretsConfiguration"] = content
            });
            var provider = new AzureKeyVaultConfigurationProvider(client) { SecretPrefix = "dev-" };

            var exception = Assert.Throws<Exception>(() => SecretsConfiguration.UseProvider(provider));

            Assert.Contains("dev-SecretsConfiguration", exception.Message);
            Assert.Contains("could not be deserialized", exception.Message);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Load_Missing_Optional_Properties_Should_Preserve_Defaults(bool perProperty)
        {
            var secretName = perProperty ? "dev-ApiKey" : "dev-SecretsConfiguration";
            var client = new InMemorySecretClient(new Dictionary<string, string>
            {
                [secretName] = perProperty ? "mock-api-key" : @"{ ""ApiKey"": ""mock-api-key"" }"
            });
            var provider = new AzureKeyVaultConfigurationProvider(client)
            {
                SecretPrefix = "dev-",
                UsePerPropertySecrets = perProperty
            };

            SecretsConfiguration.UseProvider(provider);
            var configuration = SecretsConfiguration.Instance;

            Assert.Equal("mock-api-key", configuration.ApiKey);
            Assert.Null(configuration.Smtp);
            Assert.Equal(3, configuration.RetryCount);
            Assert.Equal(1.5f, configuration.TimeoutSeconds);
            Assert.True(configuration.Enabled);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Load_Missing_Secrets_Without_Required_Properties_Should_Return_Defaults(bool perProperty)
        {
            var client = new InMemorySecretClient();
            var provider = new AzureKeyVaultConfigurationProvider(client) { UsePerPropertySecrets = perProperty };

            OptionalSecretsConfiguration.UseProvider(provider);
            var configuration = OptionalSecretsConfiguration.Instance;

            Assert.Equal("fallback-api-key", configuration.ApiKey);
            Assert.Equal(3, configuration.RetryCount);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Load_Missing_Required_Secret_Should_Fail_Validation(bool perProperty)
        {
            var client = new InMemorySecretClient();
            var provider = new AzureKeyVaultConfigurationProvider(client) { UsePerPropertySecrets = perProperty };

            var exception = Assert.Throws<Exception>(() => SecretsConfiguration.UseProvider(provider));

            Assert.Contains("API key is required.", exception.Message);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Load_Invalid_Nested_Object_Should_Fail_Validation(bool perProperty)
        {
            var secrets = perProperty
                ? new Dictionary<string, string>
                {
                    ["dev-ApiKey"] = "mock-api-key",
                    ["dev-Smtp"] = @"{ ""Username"": ""mailer@example.com"" }"
                }
                : new Dictionary<string, string>
                {
                    ["dev-SecretsConfiguration"] = @"{
                        ""ApiKey"": ""mock-api-key"",
                        ""Smtp"": { ""Username"": ""mailer@example.com"" }
                    }"
                };
            var client = new InMemorySecretClient(secrets);
            var provider = new AzureKeyVaultConfigurationProvider(client)
            {
                SecretPrefix = "dev-",
                UsePerPropertySecrets = perProperty
            };

            var exception = Assert.Throws<Exception>(() => SecretsConfiguration.UseProvider(provider));

            Assert.Contains(nameof(SecretsConfiguration.Smtp), exception.Message);
        }

        [Theory]
        [InlineData("\"sensitive-smtp-value\"")]
        [InlineData("{\"Password\":\"sensitive-smtp-value\"")]
        [InlineData("")]
        [InlineData("   ")]
        public void Load_Invalid_Property_Json_Should_Report_Name_Without_Secret_Value(string content)
        {
            var client = new InMemorySecretClient(new Dictionary<string, string>
            {
                ["dev-ApiKey"] = "mock-api-key",
                ["dev-Smtp"] = content
            });
            var provider = new AzureKeyVaultConfigurationProvider(client)
            {
                SecretPrefix = "dev-",
                UsePerPropertySecrets = true
            };

            var exception = Assert.Throws<InvalidOperationException>(() => SecretsConfiguration.UseProvider(provider));

            Assert.Contains("dev-Smtp", exception.Message);
            Assert.Contains(typeof(SmtpSecrets).FullName ?? string.Empty, exception.Message);
            Assert.DoesNotContain("sensitive-smtp-value", exception.ToString());
            Assert.Null(exception.InnerException);
        }

        [Theory]
        [InlineData("RetryCount")]
        [InlineData("TimeoutSeconds")]
        [InlineData("Enabled")]
        public void Load_Invalid_Scalar_Value_Should_Report_Name_Without_Secret_Value(string propertyName)
        {
            var secretName = "dev-" + propertyName;
            var client = new InMemorySecretClient(new Dictionary<string, string>
            {
                ["dev-ApiKey"] = "mock-api-key",
                [secretName] = "\"sensitive-invalid-value\""
            });
            var provider = new AzureKeyVaultConfigurationProvider(client)
            {
                SecretPrefix = "dev-",
                UsePerPropertySecrets = true
            };

            var exception = Assert.Throws<InvalidOperationException>(() => SecretsConfiguration.UseProvider(provider));

            Assert.Contains(secretName, exception.Message);
            Assert.DoesNotContain("sensitive-invalid-value", exception.ToString());
            Assert.Null(exception.InnerException);
        }

        [Theory]
        [InlineData(false, 403, "Forbidden")]
        [InlineData(true, 403, "Forbidden")]
        [InlineData(false, 404, "VaultNotFound")]
        [InlineData(true, 404, "VaultNotFound")]
        [InlineData(false, 500, "InternalServerError")]
        [InlineData(true, 500, "InternalServerError")]
        public void Load_Key_Vault_Failures_Should_Not_Be_Treated_As_Missing_Secrets(bool perProperty, int status, string errorCode)
        {
            var failure = new RequestFailedException(status, "Mock Key Vault failure.", errorCode, null);
            var client = new InMemorySecretClient { Failure = failure };
            var provider = new AzureKeyVaultConfigurationProvider(client) { UsePerPropertySecrets = perProperty };

            var exception = Assert.Throws<RequestFailedException>(() => SecretsConfiguration.UseProvider(provider));

            Assert.Same(failure, exception);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Reload_Should_Refresh_Secrets_When_ReloadOnAccess_Is_False(bool perProperty)
        {
            var secretName = perProperty ? "dev-ApiKey" : "dev-SecretsConfiguration";
            var secrets = new Dictionary<string, string>
            {
                [secretName] = perProperty ? "original-key" : @"{ ""ApiKey"": ""original-key"" }"
            };
            var client = new InMemorySecretClient(secrets);
            var provider = new AzureKeyVaultConfigurationProvider(client)
            {
                SecretPrefix = "dev-",
                UsePerPropertySecrets = perProperty,
                ReloadOnAccess = false
            };
            SecretsConfiguration.UseProvider(provider);
            var original = SecretsConfiguration.Instance;
            secrets[secretName] = perProperty ? "updated-key" : @"{ ""ApiKey"": ""updated-key"" }";

            Assert.Same(original, SecretsConfiguration.Instance);
            Assert.Equal("original-key", SecretsConfiguration.Instance.ApiKey);

            SecretsConfiguration.Reload();

            Assert.Equal("updated-key", SecretsConfiguration.Instance.ApiKey);
            Assert.NotSame(original, SecretsConfiguration.Instance);
            Assert.Equal(2, client.RequestedSecrets.Count(name => name == secretName));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void ReloadOnAccess_Should_Refresh_Secrets_On_Each_Access(bool perProperty)
        {
            var secretName = perProperty ? "dev-ApiKey" : "dev-SecretsConfiguration";
            var secrets = new Dictionary<string, string>
            {
                [secretName] = perProperty ? "original-key" : @"{ ""ApiKey"": ""original-key"" }"
            };
            var client = new InMemorySecretClient(secrets);
            var provider = new AzureKeyVaultConfigurationProvider(client)
            {
                SecretPrefix = "dev-",
                UsePerPropertySecrets = perProperty,
                ReloadOnAccess = true
            };
            SecretsConfiguration.UseProvider(provider);

            secrets[secretName] = perProperty ? "updated-key" : @"{ ""ApiKey"": ""updated-key"" }";
            Assert.Equal("updated-key", SecretsConfiguration.Instance.ApiKey);

            secrets[secretName] = perProperty ? "latest-key" : @"{ ""ApiKey"": ""latest-key"" }";
            Assert.Equal("latest-key", SecretsConfiguration.Instance.ApiKey);

            Assert.Equal(3, client.RequestedSecrets.Count(name => name == secretName));
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void Reload_Removed_Optional_Secrets_Should_Reset_Previous_Values(bool perProperty)
        {
            var secrets = perProperty
                ? new Dictionary<string, string>
                {
                    ["dev-ApiKey"] = "mock-api-key",
                    ["dev-Smtp"] = @"{ ""Password"": ""mock-password"" }",
                    ["dev-RetryCount"] = "7"
                }
                : new Dictionary<string, string>
                {
                    ["dev-SecretsConfiguration"] = @"{
                        ""ApiKey"": ""mock-api-key"",
                        ""Smtp"": { ""Password"": ""mock-password"" },
                        ""RetryCount"": 7
                    }"
                };
            var client = new InMemorySecretClient(secrets);
            var provider = new AzureKeyVaultConfigurationProvider(client)
            {
                SecretPrefix = "dev-",
                UsePerPropertySecrets = perProperty,
                ReloadOnAccess = false
            };
            SecretsConfiguration.UseProvider(provider);
            Assert.NotNull(SecretsConfiguration.Instance.Smtp);
            Assert.Equal(7, SecretsConfiguration.Instance.RetryCount);

            if (perProperty)
            {
                secrets.Remove("dev-Smtp");
                secrets.Remove("dev-RetryCount");
            }
            else
            {
                secrets["dev-SecretsConfiguration"] = @"{ ""ApiKey"": ""mock-api-key"" }";
            }

            SecretsConfiguration.Reload();

            Assert.Equal("mock-api-key", SecretsConfiguration.Instance.ApiKey);
            Assert.Null(SecretsConfiguration.Instance.Smtp);
        }

        [Fact]
        public void Update_Should_Throw_Without_Accessing_Key_Vault()
        {
            var client = new InMemorySecretClient();
            var provider = new AzureKeyVaultConfigurationProvider(client);

            var exception = Assert.Throws<MissingMethodException>(() => provider.Update(new SecretsConfiguration()));

            Assert.Equal("Cannot update configuration key vault", exception.Message);
            Assert.Empty(client.RequestedSecrets);
        }
    }
}
