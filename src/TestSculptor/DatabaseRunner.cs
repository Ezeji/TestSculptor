using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestSculptor.Databases.Traditional;

namespace TestSculptor
{
    /// <summary>
    /// TestSculptor main entry point
    /// </summary>
    public class DatabaseRunner
    {
        /// <summary>
        /// Specific database engine to be configured
        /// </summary>
        public DatabaseEngine DatabaseEngine { get; set; }

        /// <summary>
        /// Connection string that should be used to establish a connection to the selected database engine
        /// </summary>
        public string? ConnectionString { get; set; }

        /// <summary>
        /// Set this to true if the selected database should be configured for every test session
        /// </summary>
        public bool ShouldConfigureDatabaseAlways { get; set; }

        public void Start()
        {
            if (string.IsNullOrEmpty(ConnectionString))
            {
                throw new ArgumentNullException(nameof(ConnectionString), "Connection string must be provided.");
            }

            try
            {
                switch (DatabaseEngine)
                {
                    case DatabaseEngine.SqlServer:
                        SqlServerDatabase.ConfigureDatabase(ShouldConfigureDatabaseAlways, ConnectionString);
                        break;
                    
                    default:
                        throw new NotSupportedException($"Database engine '{DatabaseEngine}' is not supported.");
                }
            }
            catch (Exception ex)
            {
                throw new DbConfigurationFailedException(
                    $"The '{DatabaseEngine}' database engine with connection string '{ConnectionString}' can't be configured.",
                    ex);
            }
        }

        public void Stop()
        {
            try
            {
                switch (DatabaseEngine)
                {
                    case DatabaseEngine.SqlServer:
                        SqlServerDatabase.CloseDatabaseConnections();
                        break;
                    
                    default:
                        throw new NotSupportedException($"Database engine '{DatabaseEngine}' is not supported.");
                }
            }
            catch (Exception ex)
            {
                throw new DbConfigurationFailedException(
                    $"The active connections to the database '{TestSculptorDefaults.DatabaseName}' in '{DatabaseEngine}' database engine can't be closed.",
                    ex);
            }
        }
    }
}
