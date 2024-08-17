using TestSculptor;
using Microsoft.Extensions.DependencyInjection;
using TestApp.Repository;
using TestApp.Services;
using Microsoft.EntityFrameworkCore;
using TestApp.Data;

namespace TestAppTests.IntegrationTests
{
    public class TestAppSharedFixture : SharedFixture
    {
        public TestAppSharedFixture()
        {
            ConnectionString = $"Server=DESKTOP-MC1BDE5;Database={TestSculptorDefaults.DatabaseName};Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"; // Set your preferred connection string here
            DatabaseEngine = DatabaseEngine.SqlServer;  // Set your preferred database engine here
            ShouldConfigureDatabaseAlways = false;
            ConfigureServices = services =>
            {
                switch (DatabaseEngine)
                {
                    case DatabaseEngine.SqlServer:
                        services.AddDbContext<TestAppDbContext>(options =>
                                options.UseSqlServer(ConnectionString));
                        break;

                    default:
                        break;
                }

                // Configure your services and repositories here
                services.AddCoreRepository();
                services.AddServices();
            };
        }
    }
}