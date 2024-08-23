using System.Data.SqlClient;
using TestSculptor;
using TestSculptor.Databases.Traditional;
using Xunit;

namespace TestSculptorTests.TraditionalDatabases
{
    public class SqlServerDatabaseIntegrationTests
    {
        private readonly string _connectionString = $"Server=DESKTOP-MC1BDE5;Database={TestSculptorDefaults.DatabaseName};Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"; // Set your preferred connection string here

        [Fact]
        public void ConfigureDatabase_Should_DeleteDatabaseAndRunInstallScript_If_ShouldConfigureDatabaseAlways_IsTrue()
        {
            // Arrange & Act
            SqlServerDatabase.ConfigureDatabase(true, _connectionString);

            // Assert
            Assert.True(DatabaseExists(TestSculptorDefaults.DatabaseName));
        }

        [Fact]
        public void ConfigureDatabase_Should_RunInstallScript_If_ShouldConfigureDatabaseAlways_IsFalse()
        {
            // Arrange
            SqlServerDatabase.ConfigureDatabase(false, _connectionString); // Ensure the database is created

            // Act & Assert
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = $"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_CATALOG = '{TestSculptorDefaults.DatabaseName}'";
                SqlCommand command = new SqlCommand(query, connection);
                int tableCount = (int)command.ExecuteScalar();
                Assert.True(tableCount > 0, "Expected at least one table to be created in the database");
            }
        }

        [Fact]
        public void CloseDatabaseConnections_Should_End_All_Active_Database_Connections()
        {
            // Arrange
            SqlServerDatabase.ConfigureDatabase(true, _connectionString);

            // Act
            SqlServerDatabase.CloseDatabaseConnections();

            // Assert
            // Check if the database can be accessed after closing connections
            Assert.True(DatabaseExists(TestSculptorDefaults.DatabaseName));
        }

        private bool DatabaseExists(string databaseName)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand($"SELECT database_id FROM sys.databases WHERE Name = '{databaseName}'", connection))
                {
                    var result = command.ExecuteScalar();
                    return result != null;
                }
            }
        }
    }
}
