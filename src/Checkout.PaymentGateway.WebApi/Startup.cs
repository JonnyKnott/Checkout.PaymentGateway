using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Checkout.PaymentGateway.Data.Repository;
using Checkout.PaymentGateway.Models;
using Checkout.PaymentGateway.Models.ApiModels.Payment;
using Checkout.PaymentGateway.Models.Configuration;
using Checkout.PaymentGateway.Models.ServiceModels;
using Checkout.PaymentGateway.Services;
using Checkout.PaymentGateway.Services.External.Clients;
using Checkout.PaymentGateway.Services.Payment;
using Checkout.PaymentGateway.Services.Validators;
using Checkout.PaymentGateway.WebApi.Services;
using Checkout.PaymentGateway.WebApi.Services.JsonConverters;
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
        private readonly IWebHostEnvironment _webHostEnvironment;

        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionConfiguration = _configuration.GetSection("Connections").Get<ConnectionConfiguration>();
            
            services
                .AddScoped<IPaymentService, PaymentService>()
                .AddSingleton<IRequestValidator<PaymentRequest>, PaymentRequestValidator>()
                .AddSingleton<IBankRequestClient, MockBankRequestClient>()
                .AddSingleton<IPaymentExecutionService, PaymentExecutionService>()
                .AddSingleton(new DynamoDbConfiguration<PaymentResult>
                {
                    PartitionKey = Constants.DynamoDb.PaymentResultPartitionKey,
                    SortKey = Constants.DynamoDb.PaymentResultSortKey,
                    TableName = Constants.DynamoDb.PaymentResultTableName
                })
                .AddSingleton<IDynamoDbRepository<PaymentResult>, DynamoDbRepository<PaymentResult>>();
            
            services.AddControllers();

            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
            });
            
            services.AddMvc().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.Converters.Add(new PaymentAmountJsonConverter());
                options.JsonSerializerOptions.IgnoreNullValues = true;
            });

            services.AddLogging(builder => { builder.AddConsole(); });

            services.AddHealthChecks();

            if (_webHostEnvironment.IsDevelopment())
            {
                services.AddDefaultAWSOptions(new AWSOptions());

                var dynamoDbClient = new AmazonDynamoDBClient(
                    new BasicAWSCredentials(
                        _configuration["AWS_ACCESS_KEY_ID"], 
                        _configuration["AWS_SECRET_ACCESS_KEY"])
                    , new AmazonDynamoDBConfig
                {
                    UseHttp = true,
                    ServiceURL = connectionConfiguration.DynamoDb
                });
                
                services
                    .AddSingleton<IAmazonDynamoDB>(dynamoDbClient)
                    .AddHostedService<DevelopmentTableSetup>();
            }
            else
            {
                services
                    .AddAWSService<IAmazonDynamoDB>();

            }
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