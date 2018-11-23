using Signals.Aspects.Storage;
using Signals.Aspects.Storage.Database;
using Signals.Aspects.Storage.Database.Configurations;
using Signals.Tests.Storage.Helpers;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Signals.Tests.Storage
{
    public class DatabaseStorageTests
    {
        private static readonly string _inputDirectoryPath = "Files";
        private static readonly string _inputFileName = "input{#}.jpg";

        private static readonly string _outputDirectoryPath = "Files";
        private static readonly string _outputFileName = "output{#}";

        private DatabaseStorageConfiguration Configuration => new DatabaseStorageConfiguration
        {
            ConnectionString = "Data Source=sql.emitknowledge.com;Initial Catalog=app.db;User Id=appusr;Password=FYGncRXGySXDz6RFNg2e"
        };

        private async Task Lock(Func<Task> action)
        {
            lock (_inputFileName)
            {
                action();
            }
        }

        [Fact]
        public async void FileAsStream_Stored_Exists()
        {
            await Lock(async () =>
            {
                var inputFile = new TestFile(_inputDirectoryPath, _inputFileName, 1);
                var outputFile = new TestFile(_outputDirectoryPath, _outputFileName, 1);

                IStorageProvider storage = new DatabaseStorageProvider(Configuration);

                using (var source = new FileStream(inputFile.Path, FileMode.Open))
                {
                    await storage.Store(outputFile.Dir, outputFile.Name, source);
                    var outFile = storage.Get(outputFile.Dir, outputFile.Name);
                    Assert.NotNull(outFile);
                }

                await storage.Remove(outputFile.Dir, outputFile.Name);
            });
        }

        [Fact]
        public async void FileAsBytes_Stored_Exists()
        {
            await Lock(async () =>
            {
                var inputFile = new TestFile(_inputDirectoryPath, _inputFileName, 2);
                var outputFile = new TestFile(_outputDirectoryPath, _outputFileName, 2);

                byte[] fileData = null;

                using (var file = new FileStream(inputFile.Path, FileMode.Open))
                {
                    fileData = new byte[file.Length];
                    file.Read(fileData, 0, (int)file.Length);
                }

                IStorageProvider storage = new DatabaseStorageProvider(Configuration);

                await storage.Store(outputFile.Dir, outputFile.Name, fileData);
                var outFile = storage.Get(outputFile.Dir, outputFile.Name);
                Assert.NotNull(outFile);

                await storage.Remove(outputFile.Dir, outputFile.Name);
            });
        }

        [Fact]
        public async void FileAsPath_Stored_Exists()
        {
            await Lock(async () =>
            {
                var inputFile = new TestFile(_inputDirectoryPath, _inputFileName, 3);
                var outputFile = new TestFile(_outputDirectoryPath, _outputFileName, 3);

                IStorageProvider storage = new DatabaseStorageProvider(Configuration);

                await storage.Store(outputFile.Dir, outputFile.Name, inputFile.Path);
                var outFile = storage.Get(outputFile.Dir, outputFile.Name);
                Assert.NotNull(outFile);

                await storage.Remove(outputFile.Dir, outputFile.Name);
            });
        }

        [Fact]
        public async void File_Removed_DoesntExist()
        {
            await Lock(async () =>
            {
                var inputFile = new TestFile(_inputDirectoryPath, _inputFileName, 4);
                var outputFile = new TestFile(_outputDirectoryPath, _outputFileName, 4);

                IStorageProvider storage = new DatabaseStorageProvider(Configuration);

                await storage.Store(outputFile.Dir, outputFile.Name, inputFile.Path);
                await storage.Remove(outputFile.Dir, outputFile.Name);

                var outFile = storage.Get(outputFile.Dir, outputFile.Name);
                Assert.Null(outFile);
            });
        }

        [Fact]
        public async void File_Stored_EqualToInputFile()
        {
            await Lock(async () =>
            {
                var inputFile = new TestFile(_inputDirectoryPath, _inputFileName, 5);
                var outputFile = new TestFile(_outputDirectoryPath, _outputFileName, 5);

                IStorageProvider storage = new DatabaseStorageProvider(Configuration);
                using (var source = File.OpenRead(inputFile.Path))
                {
                    await storage.Store(outputFile.Dir, outputFile.Name, source);

                    using (var destination = storage.Get(outputFile.Dir, outputFile.Name))
                    {
                        source.Position = 0;
                        destination.Position = 0;

                        var inputData = new byte[source.Length];
                        var outputData = new byte[destination.Length];

                        source.Read(inputData, 0, (int)source.Length);
                        destination.Read(outputData, 0, (int)destination.Length);

                        Assert.Equal(source.Length, destination.Length);
                        Assert.Equal(inputData, outputData);
                    }
                }

                await storage.Remove(outputFile.Dir, outputFile.Name);
            });
        }
    }
}
