using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace TestSculptor
{
    public class SharedFixture : IAsyncLifetime
    {
        public DatabaseRunner? _databaseRunner;
        public IServiceProvider? ServiceProvider { get; private set; }

        public string? ConnectionString { get; set; }
        public DatabaseEngine DatabaseEngine { get; set; }
        public bool ShouldConfigureDatabaseAlways { get; set; }

        // Add an action to configure services
        public Action<IServiceCollection>? ConfigureServices { get; set; }

        public async Task InitializeAsync()
        {
            if (string.IsNullOrEmpty(ConnectionString))
            {
                throw new InvalidOperationException("ConnectionString must be set before initializing the SharedFixture.");
            }

            _databaseRunner = new DatabaseRunner
            {
                ConnectionString = ConnectionString,
                DatabaseEngine = DatabaseEngine,
                ShouldConfigureDatabaseAlways = ShouldConfigureDatabaseAlways
            };

            _databaseRunner.Start();

            // Create and build the host
            var host = CreateHost();
            await host.StartAsync();

            ServiceProvider = host.Services;
        }

        public async Task DisposeAsync()
        {
            _databaseRunner.Stop();
            _databaseRunner = null!;

            if (ServiceProvider is IAsyncDisposable asyncDisposable)
            {
                await asyncDisposable.DisposeAsync();
            }
            else if (ServiceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }

        private IHost CreateHost()
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // Call the configure services action
                    ConfigureServices?.Invoke(services);
                })
                .Build();
        }
    }
}