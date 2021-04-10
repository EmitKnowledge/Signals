using System.IO;
using System.Threading.Tasks;

namespace Signals.Aspects.Storage
{
    /// <summary>
    /// Storage provider
    /// </summary>
    public interface IStorageProvider
    {
        /// <summary>
        /// Retrieve stored file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        Stream Get(string path, string name);

        /// <summary>
        /// Remove stored file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        Task Remove(string path, string name);

        /// <summary>
        /// Store file from stream
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <param name="inStream"></param>
        /// <returns></returns>
        Task Store(string path, string name, Stream inStream);

        /// <summary>
        /// Store file as bytes
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        Task Store(string path, string name, byte[] data);

        /// <summary>
        /// Store file from location path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <param name="sourcePath"></param>
        /// <returns></returns>
        Task Store(string path, string name, string sourcePath);
    }
}
