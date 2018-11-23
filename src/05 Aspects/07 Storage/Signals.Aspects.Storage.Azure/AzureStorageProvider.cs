using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Signals.Aspects.Storage.Azure.Configurations;
using Signals.Aspects.Storage.Helpers;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Signals.Aspects.Storage.Azure
{
    /// <summary>
    /// Azure storage provider
    /// </summary>
    public class AzureStorageProvider : IStorageProvider
    {
        /// <summary>
        /// Azure configuration
        /// </summary>
        private AzureStorageConfiguration Configuration { get; set; }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="configuration"></param>
        public AzureStorageProvider(AzureStorageConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Retrieve stored file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Stream Get(string path, string name)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (name == null) throw new ArgumentNullException(nameof(name));

            var blobClient = GetClient();
            var container = blobClient.GetContainerReference(path.ToLowerInvariant());
            var blockBlob = container.GetBlockBlobReference(name.ToLowerInvariant());

            MemoryStream output = new MemoryStream();

            blockBlob.DownloadToStreamAsync(output).Wait();

            return Configuration.Decrypt(output);
        }

        /// <summary>
        /// Remove stored file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task Remove(string path, string name)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (name == null) throw new ArgumentNullException(nameof(name));

            var blobClient = GetClient();
            var container = blobClient.GetContainerReference(path.ToLowerInvariant());
            var blockBlob = container.GetBlockBlobReference(name.ToLowerInvariant());

            await blockBlob.DeleteIfExistsAsync();
        }

        /// <summary>
        /// Store file from stream
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <param name="inStream"></param>
        /// <returns></returns>
        public async Task Store(string path, string name, Stream inStream)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (inStream == null) throw new ArgumentNullException(nameof(inStream));

            var blobClient = GetClient();
            var container = blobClient.GetContainerReference(path.ToLowerInvariant());
            var blockBlob = container.GetBlockBlobReference(name.ToLowerInvariant());

            container.CreateIfNotExists();
            await blockBlob.UploadFromStreamAsync(Configuration.Encrypt(inStream));
        }

        /// <summary>
        /// Store file as bytes
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task Store(string path, string name, byte[] data)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (data == null) throw new ArgumentNullException(nameof(data));

            using (var stream = new MemoryStream(data))
            {
                await Store(path, name, stream);
            }
        }

        /// <summary>
        /// Store file from location path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <param name="sourcePath"></param>
        /// <returns></returns>
        public async Task Store(string path, string name, string sourcePath)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (sourcePath == null) throw new ArgumentNullException(nameof(sourcePath));

            using (var stream = new FileStream(sourcePath, FileMode.Open))
            {
                await Store(path, name, stream);
            }
        }

        /// <summary>
        /// Initialize azure client
        /// </summary>
        /// <returns></returns>
        private CloudBlobClient GetClient()
        {
            var storageAccount = CloudStorageAccount.Parse(Configuration.ConnectionString);
            var cloudBlobClient = storageAccount.CreateCloudBlobClient();

            return cloudBlobClient;
        }
    }
}
