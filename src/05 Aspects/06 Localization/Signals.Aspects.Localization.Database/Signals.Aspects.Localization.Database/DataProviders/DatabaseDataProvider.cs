using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using Signals.Aspects.Localization.Database.Configurations;
using Signals.Aspects.Localization.Entries;

namespace Signals.Aspects.Localization.Database.DataProviders
{
    public class DatabaseDataProvider : ILocalizationDataProvider
    {
        private DatabaseDataProviderConfiguration Configuration { get; }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="configuration"></param>
        public DatabaseDataProvider(DatabaseDataProviderConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// Loads the localization entries
        /// </summary>
        /// <returns></returns>
        public List<LocalizationEntry> LoadLocalizationEntries()
        {
            using (var connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();
                var sql =
                    $@"
                        SELECT le.Value AS leValue, le.Id as leId, le.*, coll.Name AS collName, coll.*, cat.Name AS catName,  cat.*, k.Name AS kName, k.*, lang.Name AS langName, lang.Value AS langValue, lang.*
                        FROM dbo.[{Configuration.LocalizationEntryTableName}] le
                        JOIN dbo.[{Configuration.LocalizationCollectionTableName}] coll ON le.LocalizationCollectionId = coll.Id
                        JOIN dbo.[{Configuration.LocalizationCategoryTableName}] cat ON coll.LocalizationCategoryId = cat.Id
                        JOIN dbo.[{Configuration.LocalizationKeyTableName}] k ON le.LocalizationKeyId = k.Id
                        JOIN dbo.[{Configuration.LocalizationLanguageTableName}] lang ON le.LocalizationLanguageId = lang.Id
                    ";
                var command = new SqlCommand(sql, connection);

                var entries = new List<LocalizationEntry>();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        entries.Add(new LocalizationEntry
                        {
                            Id = (int)reader["leId"],
                            Value = reader["leValue"].ToString(),
                            LocalizationLanguageId = (int)reader["LocalizationLanguageId"],
                            LocalizationLanguage = new LocalizationLanguage(
                                (int)reader["LocalizationLanguageId"],
                                reader["langName"].ToString(),
                                reader["langValue"].ToString()),
                            LocalizationCollectionId = (int)reader["LocalizationCollectionId"],
                            LocalizationCollection = new LocalizationCollection
                                ((int)reader["LocalizationCollectionId"],
                                reader["collName"].ToString(),
                                (int)reader["LocalizationCategoryId"],
                                new LocalizationCategory((int)reader["LocalizationCategoryId"], reader["catName"].ToString())),
                            LocalizationKeyId = (int)reader["LocalizationKeyId"],
                            LocalizationKey = new LocalizationKey((int)reader["LocalizationKeyId"], reader["kName"].ToString())
                        });
                    }
                }

                return entries;
            }
        }

        /// <summary>
        /// Loads the localization collections
        /// </summary>
        /// <returns></returns>
        public List<LocalizationCollection> LoadLocalizationCollections()
        {
            using (var connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();
                var sql =
                    $@"
                        SELECT coll.Name AS collName, coll.Id AS collId, coll.*, cat.Name AS catName, cat.Id AS catId
                        FROM dbo.[{Configuration.LocalizationCollectionTableName}] coll
                        JOIN dbo.[{Configuration.LocalizationCategoryTableName}] cat ON coll.LocalizationCategoryId = cat.Id
                    ";
                var command = new SqlCommand(sql, connection);

                var collections = new List<LocalizationCollection>();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        collections.Add(new LocalizationCollection
                            (
                                (int)reader["collId"],
                                reader["collName"].ToString(),
                                (int)reader["LocalizationCategoryId"],
                                new LocalizationCategory((int)reader["LocalizationCategoryId"], reader["catName"].ToString())
                            ));
                    }
                }

                return collections;
            }
        }

        /// <summary>
        /// Loads the localization categories
        /// </summary>
        /// <returns></returns>
        public List<LocalizationCategory> LoadLocalizationCategories()
        {
            using (var connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();
                var sql = $"SELECT * FROM dbo.[{Configuration.LocalizationCategoryTableName}]";
                var command = new SqlCommand(sql, connection);

                var categories = new List<LocalizationCategory>();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        categories.Add(new LocalizationCategory((int)reader["Id"], reader["Name"].ToString()));
                    }
                }

                return categories;
            }
        }

        /// <summary>
        /// Loads the localizaiton keys
        /// </summary>
        /// <returns></returns>
        public List<LocalizationKey> LoadLocalizationKeys()
        {
            using (var connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();
                var sql = $"SELECT * FROM dbo.[{Configuration.LocalizationKeyTableName}]";
                var command = new SqlCommand(sql, connection);

                var keys = new List<LocalizationKey>();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        keys.Add(new LocalizationKey((int)reader["Id"], reader["Name"].ToString()));
                    }
                }

                return keys;
            }
        }

        /// <summary>
        /// Loads the localization languages
        /// </summary>
        /// <returns></returns>
        public List<LocalizationLanguage> LoadLocalizationLanguages()
        {
            using (var connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();
                var sql = $"SELECT * FROM dbo.[{Configuration.LocalizationLanguageTableName}]";
                var command = new SqlCommand(sql, connection);

                var languages = new List<LocalizationLanguage>();

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        languages.Add(new LocalizationLanguage((int)reader["Id"], reader["Name"].ToString(), reader["Value"].ToString()));
                    }
                }

                return languages;
            }
        }

        /// <summary>
        /// Inserts new localization key
        /// </summary>
        /// <param name="localizationKey"></param>
        public void InsertLocalizationKey(LocalizationKey localizationKey)
        {
            using (var connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();
                var sql = $"INSERT INTO dbo.[{Configuration.LocalizationKeyTableName}]([Name]) VALUES(@KeyName)";
                var command = new SqlCommand(sql, connection);
                command.Parameters.Add("KeyName", SqlDbType.NVarChar);
                command.Parameters["KeyName"].Value = localizationKey.Name;

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Inserts new localization language
        /// </summary>
        /// <param name="localizationLanguage"></param>
        public void InsertLocalizationLanguage(LocalizationLanguage localizationLanguage)
        {
            using (var connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();
                var sql =
                    $@"
                        INSERT INTO dbo.[{Configuration.LocalizationLanguageTableName}]([Name], [Value]) 
                        VALUES(@LangName, @LangValue)
                    ";
                var command = new SqlCommand(sql, connection);
                command.Parameters.Add("LangName", SqlDbType.NVarChar);
                command.Parameters["LangName"].Value = localizationLanguage.Name;
                command.Parameters.Add("LangValue", SqlDbType.NVarChar);
                command.Parameters["LangValue"].Value = localizationLanguage.Value;

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Inserts new localization collection
        /// </summary>
        /// <param name="localizationCollection"></param>
        public void InsertOrUpdateLocalizationCollection(LocalizationCollection localizationCollection)
        {
            using (var connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();
                var sql =
                    $@"
                        IF EXISTS
                        (
                            SELECT *
                            FROM dbo.[{Configuration.LocalizationCollectionTableName}]
                            WHERE Id = {localizationCollection.Id}
                        )
                        UPDATE dbo.[{Configuration.LocalizationCollectionTableName}]
                        SET
                            Name = @CollName,
                            LocalizationCategoryId = {localizationCollection.LocalizationCategoryId}
                        ELSE
                        INSERT INTO dbo.[{Configuration.LocalizationCollectionTableName}]([Name], [LocalizationCategoryId]) 
                        VALUES(@CollName, {localizationCollection.LocalizationCategoryId})
                    ";
                var command = new SqlCommand(sql, connection);
                command.Parameters.Add("CollName", SqlDbType.NVarChar);
                command.Parameters["CollName"].Value = localizationCollection.Name;

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Inserts new localization entry
        /// </summary>
        /// <param name="entry"></param>
        public void InsertLocalizationEntry(LocalizationEntry entry)
        {
            using (var connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();
                var sql =
                    $@"
                        INSERT INTO dbo.[{Configuration.LocalizationEntryTableName}]
                        ([Value], LocalizationCollectionId, LocalizationKeyId, LocalizationLanguageId) 
                        VALUES(@Entry, {entry.LocalizationCollectionId}, {entry.LocalizationKeyId}, {entry.LocalizationLanguageId})
                    ";
                var command = new SqlCommand(sql, connection);
                command.Parameters.Add("Entry", SqlDbType.NVarChar);
                command.Parameters["Entry"].Value = entry.Value;

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Inserts new localization category
        /// </summary>
        /// <param name="localizationCategory"></param>
        public void InsertOrUpdateLocalizationCategory(LocalizationCategory localizationCategory)
        {
            using (var connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();
                var sql =
                    $@"
                        IF EXISTS
                        (
                            SELECT *
                            FROM dbo.[{Configuration.LocalizationCategoryTableName}]
                            WHERE Id = {localizationCategory.Id}
                        )
                        UPDATE dbo.[{Configuration.LocalizationCategoryTableName}]
                        SET
                            Name = @CatName
                        ELSE
                        INSERT INTO dbo.[{Configuration.LocalizationCategoryTableName}]([Name]) 
                        VALUES(@CatName)
                    ";
                var command = new SqlCommand(sql, connection);
                command.Parameters.Add("CatName", SqlDbType.NVarChar);
                command.Parameters["CatName"].Value = localizationCategory.Name;

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Updates an existing localizaiton entry
        /// </summary>
        /// <param name="entry"></param>
        public void UpdateLocalizationEntry(LocalizationEntry entry)
        {
            using (var connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();
                var sql =
                    $@"
                        UPDATE dbo.[{Configuration.LocalizationEntryTableName}]
                        SET
                            [Value] = @Entry,
                            [LocalizationLanguageId] = {entry.LocalizationLanguageId},
                            [LocalizationKeyId] = {entry.LocalizationKeyId},
                            [LocalizationCollectionId] = {entry.LocalizationCollectionId}
                        WHERE [Id] = {entry.Id}
                    ";
                var command = new SqlCommand(sql, connection);
                command.Parameters.Add("Entry", SqlDbType.NVarChar);
                command.Parameters["Entry"].Value = entry.Value;

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Inserts or updates list of localization entries
        /// </summary>
        /// <param name="entries"></param>
        public void InsertOrUpdateLocalizationEnties(List<LocalizationEntry> entries)
        {
            using (var connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();

                var queryBuilder = new StringBuilder();
                foreach (var localizationEntry in entries)
                {
                    var entrySql =
                        $@"
                            IF EXISTS (SELECT * FROM dbo.[{Configuration.LocalizationEntryTableName}] le WHERE le.LocalizationKeyId = {localizationEntry.LocalizationKeyId} AND le.LocalizationLanguageId = {localizationEntry.LocalizationLanguageId})
                                UPDATE dbo.[{Configuration.LocalizationEntryTableName}]
                                SET LocalizationCollectionId = {localizationEntry.LocalizationCollectionId}, LocalizationKeyId = {localizationEntry.LocalizationKeyId}, LocalizationLanguageId = {localizationEntry.LocalizationLanguageId}, Value = @Entry
                                WHERE le.LocalizationKeyId = {localizationEntry.LocalizationKeyId} AND le.LocalizationLanguageId = {localizationEntry.LocalizationLanguageId}
                            ELSE
                                INSERT INTO dbo.[{Configuration.LocalizationEntryTableName}](LocalizationKeyId, LocalizationCollectionId, LocalizationLanguageId, Value)
                                VALUES ({localizationEntry.LocalizationKeyId}, {localizationEntry.LocalizationCollectionId}, {localizationEntry.LocalizationLanguageId}, @Entry)
                        ";
                    queryBuilder.Append(entrySql + Environment.NewLine);

                    var sql = queryBuilder.ToString();
                    var command = new SqlCommand(sql, connection);
                    command.Parameters.Add("Entry", SqlDbType.NVarChar);
                    command.Parameters["Entry"].Value = localizationEntry.Value;

                    command.ExecuteNonQuery();
                }
            }
        }

    }
}
