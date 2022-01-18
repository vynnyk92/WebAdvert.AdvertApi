using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.SimpleNotificationService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AutoMapper;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using WebAdvert.AdvertApi.HealthChecks;
using WebAdvert.AdvertApi.Mapping;
using WebAdvert.AdvertApi.Services;
using WebAdvert.AdvertApi.Settings;

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
            services.AddAutoMapper(typeof(AdvertProfile));
            services.AddOptions<SNSConfig>().Bind(Configuration.GetSection("SNSConfig"));
            var options = Configuration.GetAWSOptions();


            var amazonDynamoDB = options.CreateServiceClient<IAmazonDynamoDB>();
            var snsClient = options.CreateServiceClient<IAmazonSimpleNotificationService>();

            services.AddSingleton<IAmazonDynamoDB>(amazonDynamoDB);
            services.AddSingleton<IAmazonSimpleNotificationService>(snsClient);

            services.AddTransient<IAdvertStorageService, DynamoDbAdvertStorageService>();
            services.AddTransient<IMessagePublisher, SnsPublisher>();
            services.AddControllers();
            services.AddHealthChecks()
                .AddCheck<StorageHealthCheck>("Check", tags: new[] { "check" });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Web Adverts Api",
                    Version = "v1",
                    Contact = new OpenApiContact()
                    {
                        Name = "Test"
                    }
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Web Adverts Api");
            });
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
