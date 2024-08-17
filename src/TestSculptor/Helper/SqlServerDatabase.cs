using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace TestSculptor.Helper
{
    public static class SqlServerDatabase
    {
        private static string? ConnectionString { get; set; }

        public static void ConfigureDatabase(bool shouldConfigureDatabaseAlways, string connectionString)
        {
            ConnectionString = connectionString;

            try
            {
                if (shouldConfigureDatabaseAlways)
                {
                    DeleteDatabase();
                    RunInstallScript(connectionString, TestSculptorDefaults.DatabaseName);
                }
                else
                {
                    RunInstallScript(connectionString, TestSculptorDefaults.DatabaseName);
                }
            }
            catch (Exception ex)
            {
                throw new DbConfigurationFailedException($"An error occurred when configuring your SQL Server database: {ex.Message}");
            }
        }

        public static void DeleteDatabase()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                string dropDbScript = $@"
                    USE master;
                    IF EXISTS (SELECT name FROM sys.databases WHERE name = N'{TestSculptorDefaults.DatabaseName}')
                    BEGIN
                        ALTER DATABASE [{TestSculptorDefaults.DatabaseName}]
                        SET SINGLE_USER
                        WITH ROLLBACK IMMEDIATE;
                        DROP DATABASE [{TestSculptorDefaults.DatabaseName}];
                    END";

                using (SqlCommand command = new SqlCommand(dropDbScript, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void CloseDatabaseConnections()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand command = connection.CreateCommand();

                command.CommandText = $@"
                ALTER DATABASE [{TestSculptorDefaults.DatabaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
                ALTER DATABASE [{TestSculptorDefaults.DatabaseName}] SET MULTI_USER;";

                command.ExecuteNonQuery();
            }
        }

        private static void RunInstallScript(string connectionString, string databaseName)
        {
            string sqlCmdPath = FindSqlCmd();
            string installScriptPath = FindInstallScript();

            if (string.IsNullOrEmpty(sqlCmdPath))
            {
                throw new DbConfigurationFailedException("SQLCMD executable not found. Please ensure SQL Server tools are installed and accessible.");
            }

            if (string.IsNullOrEmpty(installScriptPath))
            {
                throw new DbConfigurationFailedException("SQL Server install script not found. Please ensure it is available and accessible.");
            }

            string scriptDirectory = Path.GetDirectoryName(installScriptPath);

            (string serverName, string username, string password) = ParseConnectionString(connectionString);

            string arguments = $"-S {serverName} -i \"{installScriptPath}\" -v DatabaseName=\"{databaseName}\" -v ScriptPath=\"{scriptDirectory}\"";

            // If SQL Server authentication is used, add username and password
            if (!string.IsNullOrEmpty(username))
            {
                arguments += $" -U {username} -P {password}";
            }

            ProcessStartInfo procStartInfo = new ProcessStartInfo(sqlCmdPath, arguments)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = scriptDirectory // Set working directory to script location
            };

            using (Process process = new Process())
            {
                process.StartInfo = procStartInfo;
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode != 0)
                {
                    throw new DbConfigurationFailedException($"SQL script execution failed. Output: {output}, Error: {error}");
                }
            }
        }

        private static string FindSqlCmd()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                string programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

                string[] possiblePaths =
                [
                    Path.Combine(programFiles, @"Microsoft SQL Server\Client SDK\ODBC\170\Tools\Binn\SQLCMD.EXE"),
                    Path.Combine(programFiles, @"Microsoft SQL Server\110\Tools\Binn\SQLCMD.EXE"),
                    Path.Combine(programFiles, @"Microsoft SQL Server\120\Tools\Binn\SQLCMD.EXE"),
                    Path.Combine(programFiles, @"Microsoft SQL Server\130\Tools\Binn\SQLCMD.EXE"),
                    Path.Combine(programFiles, @"Microsoft SQL Server\140\Tools\Binn\SQLCMD.EXE"),
                    Path.Combine(programFilesX86, @"Microsoft SQL Server\Client SDK\ODBC\170\Tools\Binn\SQLCMD.EXE"),
                    Path.Combine(programFilesX86, @"Microsoft SQL Server\110\Tools\Binn\SQLCMD.EXE"),
                    Path.Combine(programFilesX86, @"Microsoft SQL Server\120\Tools\Binn\SQLCMD.EXE"),
                    Path.Combine(programFilesX86, @"Microsoft SQL Server\130\Tools\Binn\SQLCMD.EXE"),
                    Path.Combine(programFilesX86, @"Microsoft SQL Server\140\Tools\Binn\SQLCMD.EXE"),
                ];

                foreach (string path in possiblePaths)
                {
                    if (File.Exists(path))
                    {
                        return path;
                    }
                }
            }

            // For non-Windows systems or if not found in standard locations
            return "sqlcmd";
        }

        private static string FindInstallScript()
        {
            string[] possiblePaths =
            [
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "TestSculptorScripts", "install-sqlserver.sql"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "TestSculptorScripts", "install-sqlserver.sql"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestSculptorScripts", "install-sqlserver.sql")
            ];

            foreach (string path in possiblePaths)
            {
                if (File.Exists(path))
                {
                    return path;
                }
            }

            return string.Empty;
        }

        private static (string serverName, string username, string password) ParseConnectionString(string connectionString)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString);
            return (builder.DataSource, builder.UserID, builder.Password);
        }
    }
}