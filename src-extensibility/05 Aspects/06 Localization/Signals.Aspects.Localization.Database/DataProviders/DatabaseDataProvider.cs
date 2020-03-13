using Signals.Aspects.Localization.Database.Configurations;
using Signals.Aspects.Localization.Entries;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Signals.Aspects.Localization.Database.DataProviders
{
    /// <summary>
    /// Data provider for database
    /// </summary>
    public class DatabaseDataProvider : ILocalizationDataProvider
    {
        private DatabaseDataProviderConfiguration Configuration { get; }
        private List<string> DatabaseMigrations { get; set; }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="configuration"></param>
        public DatabaseDataProvider(DatabaseDataProviderConfiguration configuration)
        {
            Configuration = configuration;
            InitDatabaseMigrations();
            CreateLocalizationsTablesIfNotExist();
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
                        FROM [{Configuration.LocalizationEntryTableName}] le
                        JOIN [{Configuration.LocalizationCollectionTableName}] coll ON le.LocalizationCollectionId = coll.Id
                        JOIN [{Configuration.LocalizationCategoryTableName}] cat ON coll.LocalizationCategoryId = cat.Id
                        JOIN [{Configuration.LocalizationKeyTableName}] k ON le.LocalizationKeyId = k.Id
                        JOIN [{Configuration.LocalizationLanguageTableName}] lang ON le.LocalizationLanguageId = lang.Id
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
                        FROM [{Configuration.LocalizationCollectionTableName}] coll
                        JOIN [{Configuration.LocalizationCategoryTableName}] cat ON coll.LocalizationCategoryId = cat.Id
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
                var sql = $"SELECT * FROM [{Configuration.LocalizationCategoryTableName}]";
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
                var sql = $"SELECT * FROM [{Configuration.LocalizationKeyTableName}]";
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
                var sql = $"SELECT * FROM [{Configuration.LocalizationLanguageTableName}]";
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
                var sql = $"INSERT INTO [{Configuration.LocalizationKeyTableName}]([Name]) VALUES(@KeyName)";
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
                        INSERT INTO [{Configuration.LocalizationLanguageTableName}]([Name], [Value]) 
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
                            FROM [{Configuration.LocalizationCollectionTableName}]
                            WHERE Id = {localizationCollection.Id}
                        )
                        UPDATE [{Configuration.LocalizationCollectionTableName}]
                        SET
                            Name = @CollName,
                            LocalizationCategoryId = {localizationCollection.LocalizationCategoryId}
                        ELSE
                        INSERT INTO [{Configuration.LocalizationCollectionTableName}]([Name], [LocalizationCategoryId]) 
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
                        INSERT INTO [{Configuration.LocalizationEntryTableName}]
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
                            FROM [{Configuration.LocalizationCategoryTableName}]
                            WHERE Id = {localizationCategory.Id}
                        )
                        UPDATE [{Configuration.LocalizationCategoryTableName}]
                        SET
                            Name = @CatName
                        ELSE
                        INSERT INTO [{Configuration.LocalizationCategoryTableName}]([Name]) 
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
                        UPDATE [{Configuration.LocalizationEntryTableName}]
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
                var counter = 0;
                foreach (var localizationEntry in entries)
                {
                    var entrySql =
                        $@"
                            IF EXISTS 
                            (
                                SELECT * 
                                FROM [{Configuration.LocalizationEntryTableName}] le 
                                WHERE le.LocalizationKeyId = {localizationEntry.LocalizationKeyId} AND 
                                    le.LocalizationLanguageId = {localizationEntry.LocalizationLanguageId}
                            )
                                UPDATE [{Configuration.LocalizationEntryTableName}]
                                SET 
                                    LocalizationCollectionId = {localizationEntry.LocalizationCollectionId}, 
                                    LocalizationKeyId = {localizationEntry.LocalizationKeyId}, 
                                    LocalizationLanguageId = {localizationEntry.LocalizationLanguageId}, 
                                    Value = @Entry{counter}
                                WHERE LocalizationKeyId = {localizationEntry.LocalizationKeyId} AND 
                                    LocalizationLanguageId = {localizationEntry.LocalizationLanguageId}

                            ELSE
                                INSERT INTO [{Configuration.LocalizationEntryTableName}]
                                    (LocalizationKeyId, LocalizationCollectionId, LocalizationLanguageId, Value)
                                VALUES 
                                    (
                                        {localizationEntry.LocalizationKeyId}, 
                                        {localizationEntry.LocalizationCollectionId}, 
                                        {localizationEntry.LocalizationLanguageId}, 
                                        @Entry{counter++}
                                    )
                        ";

                    queryBuilder.Append(entrySql + Environment.NewLine);
                }

                var sql = queryBuilder.ToString();
                var command = new SqlCommand(sql, connection);

                for (var i = 0; i < entries.Count; i++)
                {
                    command.Parameters.Add($"Entry{i}", SqlDbType.NVarChar);
                    command.Parameters[$"Entry{i}"].Value = entries[i].Value;
                }

                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Updates all localization entries
        /// </summary>
        /// <param name="entries"></param>
        public void UpdateAll(List<LocalizationEntry> entries)
        {
            using (var connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();
                var command = new SqlCommand
                {
                    Connection = connection
                };

                var queryBuilder = new StringBuilder();
                var count = 0;
                foreach (var localizationEntry in entries)
                {
                    var entrySql =
                        $@"
                            UPDATE [{Configuration.LocalizationEntryTableName}]
                            SET Value = @Entry{count}
                            WHERE Id = {localizationEntry.Id};
                        ";
                    queryBuilder.Append(entrySql + Environment.NewLine);

                    command.Parameters.Add($"Entry{count}", SqlDbType.NVarChar);
                    command.Parameters[$"Entry{count}"].Value = localizationEntry.Value ?? "";

                    count++;
                }

                command.CommandText = queryBuilder.ToString();
                command.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Ensures that tables for localization entries exist in the database
        /// </summary>
        private void CreateLocalizationsTablesIfNotExist()
        {
            using (var connection = new SqlConnection(Configuration.ConnectionString))
            {
                connection.Open();

                var sql =
                    $@"
                        IF NOT EXISTS 
                        (	
                            SELECT * 
	                        FROM sys.tables t 
	                        WHERE t.name = '{Configuration.LocalizationCategoryTableName}'
                        ) 
                        CREATE TABLE [{Configuration.LocalizationCategoryTableName}]
                        (
                            [Id] [int] IDENTITY(1,1) NOT NULL,
	                        [Name] [nvarchar](max) NOT NULL
                            CONSTRAINT [PK_{Configuration.LocalizationCategoryTableName}] PRIMARY KEY CLUSTERED 
                            (
	                            [Id] ASC
                            )WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
                        ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

                        IF NOT EXISTS 
                        (	
                            SELECT * 
	                        FROM sys.tables t 
	                        WHERE t.name = '{Configuration.LocalizationCollectionTableName}'
                        ) 
                        CREATE TABLE [{Configuration.LocalizationCollectionTableName}]
                        (
                            [Id] [int] IDENTITY(1,1) NOT NULL,
	                        [Name] [nvarchar](max) NOT NULL,
	                        [LocalizationCategoryId] [int] NOT NULL
                            CONSTRAINT [PK_{Configuration.LocalizationCollectionTableName}] PRIMARY KEY CLUSTERED 
                            (
	                            [Id] ASC
                            )WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
                        ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

                        IF NOT EXISTS 
                        (	
                            SELECT * 
	                        FROM sys.tables t 
	                        WHERE t.name = '{Configuration.LocalizationEntryTableName}'
                        )
                        BEGIN
                            CREATE TABLE [{Configuration.LocalizationEntryTableName}]
                            (
                                [Id] [int] IDENTITY(1,1) NOT NULL,
                                [CreatedOn] [datetime2](7) NOT NULL,
                                [UpdatedOn] [datetime2](7) NOT NULL,
	                            [Value] [nvarchar](max) NOT NULL,
	                            [LocalizationCollectionId] [int] NOT NULL,
	                            [LocalizationLanguageId] [int] NOT NULL,
	                            [LocalizationKeyId] [int] NOT NULL
                                CONSTRAINT [PK_{Configuration.LocalizationEntryTableName}] PRIMARY KEY CLUSTERED 
                                (
	                                [Id] ASC
                                )WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
                            ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
                            ALTER TABLE [{Configuration.LocalizationEntryTableName}] ADD  CONSTRAINT [DF_{Configuration.LocalizationEntryTableName}_CreatedOn]  DEFAULT (getutcdate()) FOR [CreatedOn]
                            ALTER TABLE [{Configuration.LocalizationEntryTableName}] ADD  CONSTRAINT [DF_{Configuration.LocalizationEntryTableName}_UpdatedOn]  DEFAULT (getutcdate()) FOR [UpdatedOn]
                        END

                        IF NOT EXISTS 
                        (	
                            SELECT * 
	                        FROM sys.tables t 
	                        WHERE t.name = '{Configuration.LocalizationKeyTableName}'
                        ) 
                        CREATE TABLE [{Configuration.LocalizationKeyTableName}]
                        (
                            [Id] [int] IDENTITY(1,1) NOT NULL,
	                        [Name] [nvarchar](max) NOT NULL
                            CONSTRAINT [PK_{Configuration.LocalizationKeyTableName}] PRIMARY KEY CLUSTERED 
                            (
	                            [Id] ASC
                            )WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
                        ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

                        IF NOT EXISTS 
                        (	
                            SELECT * 
	                        FROM sys.tables t 
	                        WHERE t.name = '{Configuration.LocalizationLanguageTableName}'
                        ) 
                        CREATE TABLE [{Configuration.LocalizationLanguageTableName}]
                        (
                            [Id] [int] IDENTITY(1,1) NOT NULL,
	                        [Name] [nvarchar](max) NOT NULL,
	                        [Value] [nvarchar](max) NOT NULL
                            CONSTRAINT [PK_{Configuration.LocalizationLanguageTableName}] PRIMARY KEY CLUSTERED 
                            (
	                            [Id] ASC
                            )WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
                        ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
                    ";

                var command = new SqlCommand(sql, connection);
                command.ExecuteNonQuery();

                ExecuteDatabaseMigrations();
            }
        }

        private void InitDatabaseMigrations()
        {
            DatabaseMigrations = new List<string>
            {
                $@"
                    IF COL_LENGTH('{Configuration.LocalizationEntryTableName}', 'CreatedOn') IS NULL
                    BEGIN
                        ALTER TABLE [{Configuration.LocalizationEntryTableName}]
                        ADD CreatedOn [datetime2](7) NULL

                        ALTER TABLE [{Configuration.LocalizationEntryTableName}]
                        ADD UpdatedOn [datetime2](7) NULL

	                    EXEC ('UPDATE [{Configuration.LocalizationEntryTableName}] SET CreatedOn = GETUTCDATE(), UpdatedOn = GETUTCDATE()')

                        ALTER TABLE [{Configuration.LocalizationEntryTableName}]
                        ALTER COLUMN CreatedOn [datetime2](7) NOT NULL

                        ALTER TABLE [{Configuration.LocalizationEntryTableName}]
                        ALTER COLUMN UpdatedOn [datetime2](7) NOT NULL

                        ALTER TABLE [{Configuration.LocalizationEntryTableName}] ADD CONSTRAINT [DF_{Configuration.LocalizationEntryTableName}_CreatedOn] DEFAULT (getutcdate()) FOR [CreatedOn]
                        ALTER TABLE [{Configuration.LocalizationEntryTableName}] ADD CONSTRAINT [DF_{Configuration.LocalizationEntryTableName}_UpdatedOn] DEFAULT (getutcdate()) FOR [UpdatedOn]
                    END
                "
            };
        }

        private void ExecuteDatabaseMigrations()
        {
            foreach (var databaseMigration in DatabaseMigrations)
            {
                using (var connection = new SqlConnection(Configuration.ConnectionString))
                {
                    connection.Open();

                    var command = new SqlCommand(databaseMigration, connection);
                    command.ExecuteNonQuery();

                    connection.Close();
                }
            }
        }
    }
}
