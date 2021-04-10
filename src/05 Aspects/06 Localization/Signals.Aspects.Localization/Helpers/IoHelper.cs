using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Signals.Aspects.Localization.Helpers
{
    /// <summary>
    /// Helper for input/output operations
    /// </summary>
    internal static class IoHelper
    {
        /// <summary>
        /// Combine two paths into one
        /// </summary>
        /// <param name="firstPath"></param>
        /// <param name="secondPath"></param>
        /// <returns></returns>
        public static string CombinePaths(string firstPath, string secondPath)
        {
            var rightSlash = firstPath.Contains("/");

            if (firstPath == null)
            {
                throw new InvalidDataException(@"First path must not be null");
            }
            if (secondPath == null)
            {
                throw new InvalidDataException(@"Second path must not be null");
            }

            var path = Path.Combine(firstPath, secondPath);
            if (rightSlash)
            {
                path = path.Replace('\\', '/');
            }

            return path;
        }

        /// <summary>
        /// Return a path without the filename
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static string GetPathWithoutFileName(string fullPath)
            => Path.GetDirectoryName(fullPath);

        /// <summary>
        /// Return a list of files that match a regex
        /// </summary>
        /// <param name="path"></param>
        /// <param name="regex"></param>
        /// <param name="searchOption"></param>
        /// <returns></returns>
        public static List<string> SearchFiles(string path, string regex, SearchOption searchOption = SearchOption.AllDirectories)
        {
            if (!Directory.Exists(path)) return new List<string>();

            var directoryInfo = new DirectoryInfo(path);
            var files = directoryInfo.GetFiles(@"*.*", SearchOption.AllDirectories)
                .Where(x => x.Name.IsMatch(regex))
                .Select(x => x.FullName)
                .ToList();

            return files;
        }

        /// <summary>
        /// Returns file's folder
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static string GetFileFolder(string file)
        {
            var directory = Path.GetDirectoryName(file);
            return directory == null ? string.Empty : new DirectoryInfo(directory).Name;
        }

        /// <summary>
        /// Return filename from a path
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static string GetFileNameFromPath(string fullPath)
        {

            if (string.IsNullOrEmpty(fullPath))
            {
                throw new InvalidDataException(@"fullPath must not be null or empty");
            }

            return Path.GetFileName(fullPath);
        }

        /// <summary>
        /// Read text file
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static string ReadTextFile(string fullPath)
            => File.ReadAllText(fullPath);
    }
}
