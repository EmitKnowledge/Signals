using Newtonsoft.Json;
using Signals.Aspects.Auditing;
using Signals.Aspects.Auditing.AuditNET;
using Signals.Aspects.Auditing.AuditNET.Configurations;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using Signals.Tests.Configuration;
using Xunit;

namespace Signals.Tests.Auditing
{
    public class AuditTests
    {
        private static BaseTestConfiguration _configuration = BaseTestConfiguration.Instance;
        
        [Fact]
        public void Audit_With_FileConfiguration_Should_Create_Valid_Audit_File()
        {
            var fileName = "test-audit-log.json";
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

            // Make sure the testing audit file does not exist
            if (File.Exists(filePath))
                File.Delete(filePath);
            
            // Audit entry setup
            var auditProvider = new AuditProvider(new FileAuditingConfiguration
            {
                DirectoryPath = AppDomain.CurrentDomain.BaseDirectory,
                FilenameBuilder = () => fileName
            });

            var auditEntry = auditProvider.Entry();
            auditEntry.Payload = "Payload";
            auditEntry.Originator = "Originator";
            auditEntry.Process = "RegisterUser";
            auditEntry.EventType = "Register user";
            auditEntry.Data.Process["ProcessInfo"] = "Some process info";
            auditEntry.Data.Source["SourceInfo"] = "Some source info";

            auditProvider.Audit(auditEntry, () =>
            {
                Thread.Sleep(100);
            });

            // Check if the audit file has been created
            Assert.True(File.Exists(filePath));

            // Read the audit file and deserialize it to audit entry
            var content = File.ReadAllText(filePath);
            var entry = JsonConvert.DeserializeObject<AuditEntry>(content);

            Assert.NotNull(entry);
            Assert.Equal("Payload", entry.Payload);
            Assert.Equal("Originator", entry.Originator);
            Assert.Equal("RegisterUser", entry.Process);
            Assert.Equal("Register user", entry.EventType);
            Assert.Equal("Some process info", entry.Data.Process["ProcessInfo"]);
            Assert.Equal("Some source info", entry.Data.Source["SourceInfo"]);

            // Clean up
            File.Delete(filePath);
        }

        [Fact]
        public void Audit_With_Existing_DatabaseConfiguration_Should_Insert_Valid_Audit_Log()
        {
            var databaseConfiguration = new DatabaseAuditingConfiguration
            {
                TableName = "AuditLogTest2",
                ConnectionString = _configuration.DatabaseConfiguration.ConnectionString
            };

            using (var connection = new SqlConnection(databaseConfiguration.ConnectionString))
            {
                connection.Open();

                // Make sure the table exists
                var createTableSql =
                    $@"
                        IF NOT EXISTS 
                        (	
                            SELECT * 
	                        FROM sys.tables t
	                        WHERE t.name = '{databaseConfiguration.TableName}'
                        ) 
                        BEGIN
                            CREATE TABLE [{databaseConfiguration.TableName}]
                            (
                                [Id] INT IDENTITY(1,1) NOT NULL, 
                                [Process] NVARCHAR(MAX),
                                [ProcessInstanceId] NVARCHAR(MAX),
                                [Payload] NVARCHAR(MAX),
                                [EventType] NVARCHAR(MAX),
                                [Originator] NVARCHAR(MAX),
                                [StartDate] datetime2(7),
                                [EndDate] datetime2(7),
                                [Data] NVARCHAR(MAX)
                            )
                        END
                        DELETE FROM [{databaseConfiguration.TableName}]
                    ";

                var command = new SqlCommand(createTableSql, connection);
                command.ExecuteNonQuery();

                var auditProvider = new AuditProvider(databaseConfiguration);

                var auditEntry = auditProvider.Entry();
                auditEntry.Originator = "Originator";
                auditEntry.Process = "RegisterUser";
                auditEntry.EventType = "Register user";
                auditEntry.Payload = "Payload";
                auditEntry.Data.Process["ProcessInfo"] = "Some process info";
                auditEntry.Data.Source["SourceInfo"] = "Some source info";

                auditProvider.Audit(auditEntry, () =>
                {
                    Thread.Sleep(100);
                });

                // Retrieve the audit from the database
                var getAuditSql =
                    $@"
                        SELECT * FROM [{databaseConfiguration.TableName}]
                    ";

                command = new SqlCommand(getAuditSql, connection);
                var reader = command.ExecuteReader();

                // Make sure there is at least one audit entry
                Assert.True(reader.HasRows);

                while (reader.Read())
                {
                    Assert.Equal("Payload", reader["Payload"].ToString());
                    Assert.Equal("Originator", reader["Originator"].ToString());
                    Assert.Equal("RegisterUser", reader["Process"].ToString());
                    Assert.Equal("Register user", reader["EventType"].ToString());

                    // Deserialize the Data object and check its values
                    var content = reader["Data"].ToString();
                    var entryData = JsonConvert.DeserializeObject<AuditEntryData>(content);

                    Assert.Equal("Some process info", entryData.Process["ProcessInfo"]);
                    Assert.Equal("Some source info", entryData.Source["SourceInfo"]);
                }
                reader.Close();

                // Cleanup
                var cleanUpSql = $@"DELETE FROM {databaseConfiguration.TableName}";
                command = new SqlCommand(cleanUpSql, connection);
                command.ExecuteNonQuery();

                connection.Close();
            }
        }

        [Fact]
        public void Audit_With_NonExisting_DatabaseConfiguration_Should_Insert_Valid_Audit_Log()
        {
            var databaseConfiguration = new DatabaseAuditingConfiguration
            {
                TableName = "AuditLogTest2",
                ConnectionString = _configuration.DatabaseConfiguration.ConnectionString
            };

            using (var connection = new SqlConnection(databaseConfiguration.ConnectionString))
            {
                connection.Open();

                // Make sure the table does not exist
                var createTableSql =
                    $@"
                        IF EXISTS 
                        (	
                            SELECT * 
	                        FROM sys.tables t
	                        WHERE t.name = '{databaseConfiguration.TableName}'
                        ) 
                        DROP TABLE [{databaseConfiguration.TableName}]
                    ";

                var command = new SqlCommand(createTableSql, connection);
                command.ExecuteNonQuery();
                
                var auditProvider = new AuditProvider(databaseConfiguration);

                var auditEntry = auditProvider.Entry();
                auditEntry.Payload = "Payload";
                auditEntry.Originator = "Originator";
                auditEntry.Process = "RegisterUser";
                auditEntry.EventType = "Register user";
                auditEntry.Data.Process["ProcessInfo"] = "Some process info";
                auditEntry.Data.Source["SourceInfo"] = "Some source info";

                auditProvider.Audit(auditEntry, () =>
                {
                    Thread.Sleep(100);
                });

                // Retrieve the audit from the database
                var getAuditSql =
                    $@"
                        SELECT * FROM [{databaseConfiguration.TableName}]
                    ";

                command = new SqlCommand(getAuditSql, connection);
                var reader = command.ExecuteReader();

                // Make sure there is at least one audit entry
                Assert.True(reader.HasRows);

                while (reader.Read())
                {
                    Assert.Equal("Payload", reader["Payload"].ToString());
                    Assert.Equal("Originator", reader["Originator"].ToString());
                    Assert.Equal("RegisterUser", reader["Process"].ToString());
                    Assert.Equal("Register user", reader["EventType"].ToString());

                    // Deserialize the Data object and check its values
                    var content = reader["Data"].ToString();
                    var entryData = JsonConvert.DeserializeObject<AuditEntryData>(content);

                    Assert.Equal("Some process info", entryData.Process["ProcessInfo"]);
                    Assert.Equal("Some source info", entryData.Source["SourceInfo"]);
                }
                reader.Close();

                // Cleanup
                var cleanUpSql = $@"DELETE FROM {databaseConfiguration.TableName}";
                command = new SqlCommand(cleanUpSql, connection);
                command.ExecuteNonQuery();

                connection.Close();
            }
        }


    }
}
