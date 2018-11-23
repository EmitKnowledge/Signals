using Signals.Aspects.Storage.Helpers;
using System.IO;
using System.Threading.Tasks;
using System;
using Signals.Aspects.Storage.File.Configurations;

namespace Signals.Aspects.Storage.File
{
    /// <summary>
    /// File storage provider
    /// </summary>
    public class FileStorageProvider : IStorageProvider
    {        
        /// <summary>
        /// File configuration
        /// </summary>
        private FileStorageConfiguration Configuration { get; set; }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="configuration">Default: <see cref="FileStorageConfiguration"></see></param>
        public FileStorageProvider(FileStorageConfiguration configuration = null)
        {
            Configuration = configuration ?? new FileStorageConfiguration();
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

            var filePath = Path.Combine(Configuration.RootPath, path, name);
            var file = new FileInfo(filePath);

            if (file.Exists)
            {
                using (var fileSteram = new FileStream(filePath, FileMode.Open))
                {
                    return Configuration.Decrypt(fileSteram);
                }
            }
            else return null;
        }

		/// <summary>
		/// Remove stored file
		/// </summary>
		/// <param name="path"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public Task Remove(string path, string name)
		{
			if (path == null) throw new ArgumentNullException(nameof(path));
			if (name == null) throw new ArgumentNullException(nameof(name));

			var filePath = Path.Combine(Configuration.RootPath, path, name);
			var file = new FileInfo(filePath);

			file.Delete();
			while (file.Exists)
			{
				System.Threading.Thread.Sleep(100);
				file.Refresh();
			}

			return Task.CompletedTask;
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

            var directoryPath = Path.Combine(Configuration.RootPath, path);
            Directory.CreateDirectory(directoryPath);

            var filePath = Path.Combine(directoryPath, name);
            using (var outStream = System.IO.File.Create(filePath))
            {
                using (var encryptStream = Configuration.Encrypt(inStream))
                {
                    await encryptStream.CopyToAsync(outStream);
                }
            }
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
    }
}
