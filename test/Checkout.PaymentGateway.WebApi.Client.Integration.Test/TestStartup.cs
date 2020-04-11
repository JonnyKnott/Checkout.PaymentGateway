using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Checkout.PaymentGateway.WebApi.Client.Payment;
using Checkout.PaymentGateway.WebApi.Client.Payment.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;
using Xunit.Abstractions;
using Xunit.DependencyInjection;

[assembly: TestFramework(
    "Checkout.PaymentGateway.WebApi.Client.Integration.Test.TestStartup",
    "Checkout.PaymentGateway.WebApi.Client.Integration.Test")]
namespace Checkout.PaymentGateway.WebApi.Client.Integration.Test
{
    public class TestStartup : DependencyInjectionTestFramework
    {
        public TestStartup(IMessageSink messageSink) : base(messageSink)
        {
        }
        
        protected override IHostBuilder CreateHostBuilder(AssemblyName assemblyName) =>
            base.CreateHostBuilder(assemblyName)
                .ConfigureHostConfiguration(builder =>
                {
                    builder.AddInMemoryCollection(new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("PaymentClient:Endpoint", "http://localhost:9002/")
                    });
                })
                .ConfigureServices((context, services) => ConfigureServices(context, services));

        protected void ConfigureServices(HostBuilderContext context, IServiceCollection services)
        {
            var clientConfiguration =
                context.Configuration.GetSection("PaymentClient").Get<PaymentClientConfiguration>();

            services.AddPaymentServiceClient(clientConfiguration);
        }
    }
}