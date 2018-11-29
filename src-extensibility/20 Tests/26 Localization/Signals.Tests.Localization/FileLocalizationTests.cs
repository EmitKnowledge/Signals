using Signals.Aspects.Localization;
using Signals.Aspects.Localization.Base;
using Signals.Aspects.Localization.File.Configurations;
using Signals.Aspects.Localization.File.DataProviders;
using System;
using System.Collections.Generic;
using Xunit;

namespace Signals.Tests.Localization
{
    public class FileLocalizationTests
    {
        private readonly ILocalizationProvider _provider;

        public FileLocalizationTests()
        {
            // Create instance

            var databaseConfig = new JsonDataProviderConfiguration
            {
                DirectoryPath = Environment.CurrentDirectory,
                FileExtension = "app",
                LocalizationSources = new List<LocalizationSource>
                {
                    new LocalizationSource
                    {
                        Name = "sample",
                        UseBaseDirectory = true,
                        SourcePath = "FileSources"
                    }
                }
            };
            var databaseDataProvider = new JsonDataProvider(databaseConfig);

            _provider = new LocalizationProvider(databaseDataProvider);
        }

        [Fact]
        public void Localization_FromFile_Exists()
        {
            var message = _provider.Get("SOME_KEY");
            Assert.Equal("Some message", message.Value);
        }
    }
}