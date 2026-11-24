using Azure;
using Azure.Security.KeyVault.Secrets;
using System.Collections.Generic;
using System.Threading;

namespace Signals.Tests.Configuration.Fakes
{
    internal sealed class InMemorySecretClient : SecretClient
    {
        private readonly IDictionary<string, string> _secrets;

        public InMemorySecretClient(IDictionary<string, string> secrets = null)
        {
            _secrets = secrets ?? new Dictionary<string, string>();
        }

        public List<string> RequestedSecrets { get; } = new List<string>();

        public RequestFailedException Failure { get; set; }

        public override Response<KeyVaultSecret> GetSecret(
            string name,
            string version = null,
            SecretContentType? outContentType = null,
            CancellationToken cancellationToken = default)
        {
            RequestedSecrets.Add(name);
            if (Failure != null) throw Failure;

            if (!_secrets.TryGetValue(name, out var content))
            {
                throw new RequestFailedException(404, "Mock secret was not found.", "SecretNotFound", null);
            }

            // The provider only reads the value, not the underlying HTTP response.
            return Response.FromValue(new KeyVaultSecret(name, content), null);
        }
    }
}
