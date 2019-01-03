using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UrlShortener.Authentication;
using UrlShortener.Services;

namespace UrlShortener
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            // Set up auth.
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CustomAuthOptions.DefaultScheme;
                options.DefaultChallengeScheme = CustomAuthOptions.DefaultScheme;
            }).AddCustomAuth();

            // Init MVC.
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // Add dependent services.
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Add DynamoDB to the ASP.NET Core dependency injection framework.
            services.AddAWSService<IAmazonDynamoDB>();

            // Add custom services.
            services.AddTransient<IConfigHelper, ConfigHelper>();
            services.AddTransient<IAuther, Auther>();
#if DEBUG
            services.AddTransient<IForwardDb, ForwardDbDev>();
#else
            services.AddTransient<IForwardDb, ForwardDb>();
#endif
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            // Enable auth.
            app.UseAuthentication();

            // Use status pages.
            app.UseStatusCodePages(context =>
            {
                var resp = context.HttpContext.Response;

                if (resp.StatusCode == StatusCodes.Status401Unauthorized)
                {
                    resp.Redirect("/");
                }
                return Task.CompletedTask;
            });

            app.UseStaticFiles();
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}
