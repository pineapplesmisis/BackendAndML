using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CheckersBackend.Data;
using MCH.BackgroundServices;
using MCH.Configuration;
using MCH.Core.Parsing;
using MCH.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MCH
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IParsingRepository, ParsingRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddHostedService<Worker>();
            var appSettings = _configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettings);
           
            services.AddDbContext<ProductionInfoDbContext>(options => options.UseNpgsql(appSettings.Get<AppSettings>().DbConnection, x => x.MigrationsAssembly("MCH.API")));
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context => { await context.Response.WriteAsync("Hello World!"); });
            });
        }
    }
}