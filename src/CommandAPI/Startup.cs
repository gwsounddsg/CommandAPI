using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using CommandAPI.Models;


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
            // connects our CommandContext to our db
            services.AddDbContext<CommandContext>(opt => 
            {
                opt.UseNpgsql(Configuration.GetConnectionString("PostgreSqlConnection"));
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
