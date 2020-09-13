using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using CommandAPI.Models;
using Npgsql;


namespace CommandAPI
{
    public class Startup
    {
        public IConfiguration Configuration { get; }


        // this gives up access to our appsettings.json
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        public void ConfigureServices(IServiceCollection services)
        {
            // gets username and password from secret file
            var builder = new NpgsqlConnectionStringBuilder()
            {
                ConnectionString = Configuration.GetConnectionString("PostgreSqlConnection"),
                Username = Configuration["UserID"],
                Password = Configuration["Password"]
            };

            // connects our CommandContext to our db
            services.AddDbContext<CommandContext>(opt => 
            {
                opt.UseNpgsql(builder.ConnectionString);
            });
            
            services.AddControllers();
        }

        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
