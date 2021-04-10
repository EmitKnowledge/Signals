using Signals.Core.Common.Instance;
using Signals.Core.Common.Regexes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Signals.Core.Common.InputOutput
{
    /// <summary>
    /// Helper for input/output operations
    /// </summary>
    public static class IoHelper
    {
        /// <summary>
        /// Read byte file
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static byte[] ReadByteFile(string fullPath)
        {
            var _file = File.ReadAllBytes(fullPath);
            return _file;
        }

        /// <summary>
        /// Read byte file
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static string ReadFile(string fullPath)
        {
            var _file = File.ReadAllText(fullPath);
            return _file;
        }

        /// <summary>
        /// Read byte file
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static List<string> ReadLines(string fullPath)
        {
            var isExistingFile = File.Exists(fullPath);
            if (!isExistingFile) return new List<string>();

            var lines = new List<string>();
            using (var fs = new FileStream(fullPath,
                                                  FileMode.Open,
                                                  FileAccess.Read,
                                                  FileShare.ReadWrite))
            {
                using (var sr = new StreamReader(fs))
                {
                    while (sr.Peek() >= 0) // reading the old data
                    {
                        var line = sr.ReadLine();
                        lines.Add(line);
                    }
                }
            }
            return lines;
        }

        /// <summary>
        /// Return lines from directory files
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns></returns>
        public static List<string> ReadLinesFromDirectory(string directoryPath)
        {
            var isExistingDirectory = Directory.Exists(directoryPath);
            if (!isExistingDirectory) return new List<string>();
            var sortedFiles = new DirectoryInfo(directoryPath).GetFiles()
                                                              .OrderBy(f => f.LastWriteTime)
                                                              .ToList();
            var lines = new List<string>();
            foreach (var file in sortedFiles)
            {
                var data = ReadLines(file.FullName);
                lines.AddRange(data);
            }
            return lines;
        }

        /// <summary>
        /// Create directory at a given path
        /// </summary>
        /// <param name="fullPath"></param>
        public static void CreateDirectory(string fullPath)
        {
            Directory.CreateDirectory(fullPath);
        }

        /// <summary>
        /// Create directory at a given path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="directoryName"></param>
        public static void CreateDirectory(string path, string directoryName)
        {
            Directory.CreateDirectory(CombinePaths(path, directoryName));
        }

        /// <summary>
        /// Read text file
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static string ReadTextFile(string fullPath)
        {
            var _file = File.ReadAllText(fullPath);
            return _file;
        }

        /// <summary>
        /// Write text file
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static void WriteTextFile(string fullPath, string content)
        {
            File.WriteAllText(fullPath, content);
        }

        /// <summary>
        /// Check if a path is a file
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static bool IsFile(string fullPath)
        {
            var _fileAttributes = File.GetAttributes(fullPath);
            bool _isDir = (_fileAttributes & FileAttributes.Directory) == FileAttributes.Directory;
            return !_isDir;
        }

        /// <summary>
        /// Search for a file in a directory and all subdirectories
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static string FindFile(string filename, string directory)
        {
            return Directory.EnumerateFiles(directory, filename, SearchOption.AllDirectories)
                            .FirstOrDefault();
        }

        /// <summary>
        /// Return if a file existis
        /// </summary>
        /// <param name="fullpath"></param>
        /// <returns></returns>
        public static bool FileExistis(string fullpath)
        {
            try
            {
                return File.Exists(fullpath);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Return if a irectory existis
        /// </summary>
        /// <param name="fullpath"></param>
        /// <returns></returns>
        public static bool DirectoryExistis(string fullpath)
        {
            return Directory.Exists(fullpath);
        }

        /// <summary>
        /// Combine two paths into one
        /// </summary>
        /// <param name="firstPath"></param>
        /// <param name="secondPath"></param>
        /// <returns></returns>
        public static string CombinePaths(string firstPath, string secondPath)
        {
            var _rightSlash = firstPath.Contains("/");

            if (firstPath.IsNull())
            {
                throw new InvalidDataException(@"First path must not be null");
            }
            if (secondPath.IsNull())
            {
                throw new InvalidDataException(@"Second path must not be null");
            }

            var _path = Path.Combine(firstPath, secondPath);
            if (_rightSlash)
            {
                _path = _path.Replace('\\', '/');
            }

            return _path;
        }

        /// <summary>
        /// Return filename from a path
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static string GetFileNameFromPath(string fullPath)
        {
            if (fullPath.IsNullOrEmpty())
            {
                throw new InvalidDataException(@"fullPath must not be null or empty");
            }

            return Path.GetFileName(fullPath);
        }

        /// <summary>
        /// Return a path without the filename
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static string GetPathWithoutFileName(string fullPath)
        {
            var _cleanPath = Path.GetDirectoryName(fullPath);
            return _cleanPath;
        }

        /// <summary>
        /// Return filename without extension
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetFileNameWithoutExtension(string path)
        {
            return Path.GetFileNameWithoutExtension(path);
        }

        /// <summary>
        /// Return filename extension
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        public static string GetExtensionFromFileName(string fullPath)
        {
            if (fullPath.IsNullOrEmpty())
            {
                throw new InvalidDataException(@"fullPath must not be null or empty");
            }

            return Path.GetExtension(fullPath);
        }

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

            var _directoryInfo = new DirectoryInfo(path);
            // must hack it due to regex restrictions in search
            var _files = _directoryInfo.GetFiles(@"*.*", SearchOption.AllDirectories)
                                                  .Where(x => x.Name.IsMatch(regex))
                                                  .Select(x => x.FullName)
                                                  .ToList();
            return _files;
        }
    }
}