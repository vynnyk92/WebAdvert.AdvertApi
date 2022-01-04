using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AutoMapper;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using WebAdvert.AdvertApi.HealthChecks;
using WebAdvert.AdvertApi.Mapping;
using WebAdvert.AdvertApi.Services;

namespace WebAdvert.AdvertApi
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
            var options = Configuration.GetAWSOptions();

            services.AddAutoMapper(typeof(AdvertProfile));
            var amazonDynamoDB = options.CreateServiceClient<IAmazonDynamoDB>();
            services.AddSingleton<IAmazonDynamoDB>(amazonDynamoDB);
            services.AddTransient<IAdvertStorageService, DynamoDbAdvertStorageService>();
            services.AddControllers();
            services.AddHealthChecks()
                .AddCheck<StorageHealthCheck>("Check", tags: new[] { "check" });
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
                endpoints.MapHealthChecks("/health/check", new HealthCheckOptions
                {
                    Predicate = hc => hc.Tags.Contains("check")
                });
                endpoints.MapControllers();
            });
        }
    }
}
