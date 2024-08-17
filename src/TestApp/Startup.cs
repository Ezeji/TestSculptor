using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using TestApp.Data;
using TestApp.Repository;
using TestApp.Services;
using TestSculptor;

namespace TestApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddDbContext<TestAppDbContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("TestSculptorDb"), b => b.MigrationsAssembly("TestApp"));
            });

            services.AddCors(option =>
            {
                option.AddDefaultPolicy(
                    builder =>
                    {
                        builder
                        .AllowAnyOrigin()
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                    });
            });

            services.AddHttpClient();

            services.AddCoreRepository();
            services.AddServices();

            // Initialize TestSculptor for functional testing
            DatabaseRunner? _databaseRunner = new()
            {
                ConnectionString = Configuration.GetConnectionString(TestSculptorDefaults.DatabaseName),
                DatabaseEngine = DatabaseEngine.SqlServer,
                ShouldConfigureDatabaseAlways = false
            };
            _databaseRunner.Start();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Test App Service", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseSwagger();
            app.UseSwaggerUI(opt =>
            {
                opt.SwaggerEndpoint("/swagger/v1/swagger.json", "Test App Service");
            });

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
