using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Checkout.PaymentGateway.Models.ApiModels.Payment;
using Checkout.PaymentGateway.Models.Configuration;
using Checkout.PaymentGateway.Services;
using Checkout.PaymentGateway.Services.Validators;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Checkout.PaymentGateway.WebApi
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
            var connectionConfiguration = _configuration.GetSection("Connections").Get<ConnectionConfiguration>();

            services
                .AddScoped<IPaymentService, PaymentService>()
                .AddSingleton<IRequestValidator<PaymentRequest>, PaymentRequestValidator>();
            
            services.AddControllers();

            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
            });
            
            services.AddMvc().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.IgnoreNullValues = true;
            });

            services.AddLogging(builder => { builder.AddConsole(); });

            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = connectionConfiguration.RedisEndpoint;
                options.InstanceName = "master";
            });

            services.AddHealthChecks();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHealthChecks("/ping", new HealthCheckOptions
            {
                AllowCachingResponses = false,
                ResultStatusCodes = new Dictionary<HealthStatus, int>
                {
                    { HealthStatus.Healthy, StatusCodes.Status200OK },
                    { HealthStatus.Degraded, StatusCodes.Status200OK },
                    { HealthStatus.Unhealthy, StatusCodes.Status503ServiceUnavailable }

                }
            });
            
            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}