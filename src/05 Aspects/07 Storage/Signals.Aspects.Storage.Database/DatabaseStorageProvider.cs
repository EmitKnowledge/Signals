using Signals.Aspects.Storage.Helpers;
using Signals.Aspects.Storage.Database.Configurations;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;

namespace Signals.Aspects.Storage.Database
{
    /// <summary>
    /// Database storage provider
    /// </summary>
    public class DatabaseStorageProvider : IStorageProvider
    {        
        /// <summary>
        /// Database configuration
        /// </summary>
        private DatabaseStorageConfiguration Configuration { get; set; }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="configuration"></param>
        public DatabaseStorageProvider(DatabaseStorageConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        /// <summary>
        /// Retrieve stored file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Stream Get(string path, string name)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (name == null) throw new ArgumentNullException(nameof(name));

            using (var connection = GetConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "SELECT Data FROM [File] WHERE Path = @Path AND Name = @Name";

                command.Parameters.Add(new SqlParameter("@Path", path));
                command.Parameters.Add(new SqlParameter("@Name", name));

	            if (!(command.ExecuteScalar() is byte[] data)) return null;

                using (var encryptStream = Configuration.Decrypt(new MemoryStream(data)))
                {
                    data = new byte[encryptStream.Length];
                    encryptStream.Read(data, 0, data.Length);
                }

                return new MemoryStream(data);
            }
        }
        
        /// <summary>
        /// Remove stored file
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public Task Remove(string path, string name)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (name == null) throw new ArgumentNullException(nameof(name));

            using (var connection = GetConnection())
            {
                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM [File] WHERE Path = @Path AND Name = @Name";

                command.Parameters.Add(new SqlParameter("@Path", path));
                command.Parameters.Add(new SqlParameter("@Name", name));

                command.ExecuteNonQuery();

                return Task.CompletedTask;
            }
        }
        
        /// <summary>
        /// Store file from stream
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <param name="inStream"></param>
        /// <returns></returns>
        public Task Store(string path, string name, Stream inStream)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (inStream == null) throw new ArgumentNullException(nameof(inStream));

            using (var connection = GetConnection())
            {
                inStream.Position = 0;
                byte[] data;

                using (var encryptStream = Configuration.Encrypt(inStream))
                {
                    data = new byte[encryptStream.Length];
                    encryptStream.Read(data, 0, data.Length);
                }

                inStream.Position = 0;

                connection.Open();
                var command = connection.CreateCommand();
                command.CommandText = @"
                    BEGIN TRANSACTION
	                    DECLARE @Count int = (SELECT COUNT(*) FROM [File] WHERE Path = @Path AND Name = @Name)
	                    IF @Count > 0
		                    UPDATE [File] SET Path = @Path, Name = @Name, Data = @Data, IsEncrypted = @IsEncrypted
	                    ELSE
		                    INSERT INTO [File] (Path, Name, Data, IsEncrypted) VALUES (@Path, @Name, @Data, @IsEncrypted)
                    COMMIT
                ";

                command.Parameters.Add(new SqlParameter("@Path", path));
                command.Parameters.Add(new SqlParameter("@Name", name));
                command.Parameters.Add(new SqlParameter("@Data", data));
                command.Parameters.Add(new SqlParameter("@IsEncrypted", Configuration.Encrypt));
                
                command.ExecuteNonQuery();

                return Task.CompletedTask;
            }
        }
        
        /// <summary>
        /// Store file as bytes
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task Store(string path, string name, byte[] data)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (data == null) throw new ArgumentNullException(nameof(data));

            using (var stream = new MemoryStream(data))
            {
                await Store(path, name, stream);
            }
        }
        
        /// <summary>
        /// Store file from location path
        /// </summary>
        /// <param name="path"></param>
        /// <param name="name"></param>
        /// <param name="sourcePath"></param>
        /// <returns></returns>
        public async Task Store(string path, string name, string sourcePath)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (sourcePath == null) throw new ArgumentNullException(nameof(sourcePath));

            using (var stream = new FileStream(sourcePath, FileMode.Open))
            {
                await Store(path, name, stream);
            }
        }

        /// <summary>
        /// Get sql connection
        /// </summary>
        /// <returns></returns>
        private IDbConnection GetConnection()
        {
            return new SqlConnection(Configuration.ConnectionString);
        }
    }
}
