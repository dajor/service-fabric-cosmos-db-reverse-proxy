using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AuditingService.Interfaces.Repositories;
using AuditingService.Repositories;
using AuditingService.Interfaces.Logics;
using AuditingService.Logics;
using AuditingService.Core;
using CommonBase;
using CommonBase.Authentication;

namespace AuditingService
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
            
            services.AddOptions();

            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            
            services.AddMvc();

            // Register application services.
            services.AddScoped<IItemsLogic, ItemsLogic>();
            services.AddScoped<IItemsRepository, ItemsRepository>();

            IServiceProvider provider = services.BuildServiceProvider();
            
            IOptions<AppSettings> otherService = provider.GetRequiredService<IOptions<AppSettings>>();

            DocumentClientProvider documentClientProvider = new DocumentClientProvider(otherService);

            var dataAccess = new DataAccess(documentClientProvider.Client);
            services.AddSingleton<IDataAccess>(dataAccess);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseMiddleware<CertificateAuthenticationMiddleware>();
            }

            app.UseMvc();
        }
    }
}
