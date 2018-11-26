using System.Data.SqlClient;
using System.Globalization;
using Signals.Aspects.Localization;
using Signals.Aspects.Localization.Base;
using Signals.Aspects.Localization.Database.Configurations;
using Signals.Aspects.Localization.Database.DataProviders;
using Signals.Aspects.Localization.File.Configurations;
using Signals.Aspects.Localization.File.DataProviders;
using Xunit;

namespace Signals.Tests.Localization
{
    public class FileLocalizationTests
    {
        private readonly ILocalizationProvider _provider;

        public FileLocalizationTests()
        {
            // Create instance

            var databaseConfig = new JsonDataProviderConfiguration();
            var databaseDataProvider = new JsonDataProvider(databaseConfig);

            _provider = new LocalizationProvider(databaseDataProvider);
        }
    }
}
