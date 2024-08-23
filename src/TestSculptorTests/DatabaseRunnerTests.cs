using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestSculptor;
using Xunit;

namespace TestSculptorTests
{
    public class DatabaseRunnerTests
    {
        [Fact]
        public void Start_Should_Throw_ArgumentNullException_If_ConnectionString_IsNullOrEmpty()
        {
            // Arrange
            DatabaseRunner runner = new DatabaseRunner
            {
                DatabaseEngine = DatabaseEngine.SqlServer,
                ConnectionString = string.Empty,
                ShouldConfigureDatabaseAlways = false
            };

            // Act & Assert
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(() => runner.Start());
            Assert.Equal("ConnectionString", exception.ParamName);
        }

        [Fact]
        public void Stop_Should_Throw_NotSupportedException_If_DatabaseEngine_IsNotSupported()
        {
            // Arrange
            DatabaseRunner runner = new DatabaseRunner
            {
                DatabaseEngine = (DatabaseEngine)999 // An unsupported database engine
            };

            // Act & Assert
            DbConfigurationFailedException exception = Assert.Throws<DbConfigurationFailedException>(() => runner.Stop());
            Assert.Equal($"The active connections to the database '{TestSculptorDefaults.DatabaseName}' in '{runner.DatabaseEngine}' database engine can't be closed.", exception.Message);
            Assert.IsType<NotSupportedException>(exception.InnerException);
        }
    }
}
