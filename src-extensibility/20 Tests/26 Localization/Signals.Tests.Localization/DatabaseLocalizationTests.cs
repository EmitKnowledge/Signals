﻿using System.Data.SqlClient;
using System.Globalization;
using Signals.Aspects.Localization;
using Signals.Aspects.Localization.Base;
using Signals.Aspects.Localization.Database.Configurations;
using Signals.Aspects.Localization.Database.DataProviders;
using Signals.Tests.Configuration;
using Xunit;

namespace Signals.Tests.Localization
{
    public class DatabaseLocalizationTests
    {
        private static BaseTestConfiguration _configuration = BaseTestConfiguration.Instance;
        
        private static string ConnectionString = _configuration.DatabaseConfiguration.ConnectionString;

        private readonly ILocalizationProvider _provider;

        public DatabaseLocalizationTests()
        {
            // Create instance

            var databaseConfig = new DatabaseDataProviderConfiguration(ConnectionString);
            var databaseDataProvider = new DatabaseDataProvider(databaseConfig);

            _provider = new LocalizationProvider(databaseDataProvider);
        }

        [Fact]
        public void Set_New_Translation_Should_Create_Non_Existing_Entries()
        {
            lock (ConnectionString)
            {
                CleanDatabase();

                var key = "Key";
                var translation = "Translation";
                var collection = "some_collection";
                var category = "some_category";
                var culture = new CultureInfo("en-US");

                _provider.Set(key, translation, collection, category, culture);

                var insertedEntry = _provider.Get(key, null, null, culture);

                Assert.NotNull(insertedEntry);
                Assert.Equal(translation, insertedEntry.Value);

                Assert.NotNull(insertedEntry.LocalizationLanguage);
				Assert.Equal(culture.Name.ToLower(), insertedEntry.LocalizationLanguage.Name.ToLower());
				Assert.Equal(culture.DisplayName.ToLower(), insertedEntry.LocalizationLanguage.Value.ToLower());

                Assert.NotNull(insertedEntry.LocalizationKey);
                Assert.Equal(key, insertedEntry.LocalizationKey.Name);

                Assert.NotNull(insertedEntry.LocalizationCollection);
                Assert.Equal(collection, insertedEntry.LocalizationCollection.Name);

                Assert.NotNull(insertedEntry.LocalizationCollection.LocalizationCategory);
                Assert.Equal(category, insertedEntry.LocalizationCollection.LocalizationCategory.Name);

                CleanDatabase();
            }
        }

        [Fact]
        public void Set_Existing_Translation_Should_Not_Create_New_Entries()
        {
            lock (ConnectionString)
            {
                CleanDatabase();

                var key = "Key";
                var translation = "Translation";
                var collection = "some_collection";
                var category = "some_category";
                var culture = new CultureInfo("en-US");

                // This should create, since the database is clean
                _provider.Set(key, translation, collection, category, culture);

                var keys = _provider.GetAllKeys();
                var collections = _provider.GetAllCollections();
                var languages = _provider.GetAllLanguages();
                var categories = _provider.GetAllCategories();
                var entries = _provider.GetAllForCulture(culture);

                Assert.Single(keys);
                Assert.Single(collections);
                Assert.Single(languages);
                Assert.Single(categories);
                Assert.Single(entries);

                // This should update, not create
                _provider.Set(key, translation, collection, category, culture);

                keys = _provider.GetAllKeys();
                collections = _provider.GetAllCollections();
                languages = _provider.GetAllLanguages();
                categories = _provider.GetAllCategories();
                entries = _provider.GetAllForCulture(culture);

                Assert.Single(keys);
                Assert.Single(collections);
                Assert.Single(languages);
                Assert.Single(categories);
                Assert.Single(entries);

                CleanDatabase();
            }
        }

        [Fact]
        public void Set_Existing_Translation_Should_Update_Entry()
        {
            lock (ConnectionString)
            {
                CleanDatabase();

                var key = "Key";
                var translation = "Translation";
                var collection = "some_collection";
                var category = "some_category";
                var culture = new CultureInfo("en-US");

                // This should create, since the database is clean
                _provider.Set(key, translation, collection, category, culture);

                var newTranslation = "NewTranslation";
                var newCollection = "some_new_collection";
                var newCategory = "some_new_category";

                // This should update, not create
                _provider.Set(key, newTranslation, newCollection, newCategory, culture);

                var updatedEntry = _provider.Get(key, null, null, culture);

                Assert.NotNull(updatedEntry);
                Assert.Equal(newTranslation, updatedEntry.Value);

                Assert.NotNull(updatedEntry.LocalizationCollection);
                Assert.Equal(newCollection, updatedEntry.LocalizationCollection.Name);

                Assert.NotNull(updatedEntry.LocalizationCollection.LocalizationCategory);
                Assert.Equal(newCategory, updatedEntry.LocalizationCollection.LocalizationCategory.Name);

                CleanDatabase();
            }
        }

        private void CleanDatabase()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                var sql =
                    $@"
                        delete from LocalizationEntry;
                        delete from LocalizationLanguage;
                        delete from LocalizationCollection;
                        delete from LocalizationCategory;
                        delete from LocalizationKey;
                    ";

                var command = new SqlCommand(sql, connection);
                command.ExecuteNonQuery();
            }
        }
    }
}
