using Signals.Aspects.Localization;
using Signals.Aspects.Localization.Base;
using Signals.Aspects.Localization.File.Configurations;
using Signals.Aspects.Localization.File.DataProviders;
using System;
using System.Collections.Generic;
using System.Globalization;
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
            var message = _provider.Get("SOME_KEY", null, null, new CultureInfo("en"));
            Assert.Equal("Some message", message.Value);
        }

        [Fact]
        public void Localization_From_Specific_Collection_Exists()
        {
            var messageFirst = _provider.Get("SOME_KEY", "localization-strings", null, new CultureInfo("en"));
            var messageC1 = _provider.Get("SOME_KEY", "localization-strings", "Category01", new CultureInfo("en"));
            var messageC2 = _provider.Get("SOME_KEY", "localization-strings", "Category02", new CultureInfo("en"));

            Assert.Equal("Some message", messageFirst.Value);
            Assert.Equal("Message from category 01", messageC1.Value);
            Assert.Equal("Message from category 02", messageC2.Value);
        }

        [Fact]
        public void Localization_From_Specific_Category_Exists()
        {
            var messageFirst = _provider.Get("SOME_KEY", null, "Category01", new CultureInfo("en"));
            var messageSecond = _provider.Get("SOME_KEY", null, "Category02", new CultureInfo("en"));

            Assert.Equal("Message from category 01", messageFirst.Value);
            Assert.Equal("Message from category 02", messageSecond.Value);
        }
    }
}