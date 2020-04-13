using System.Collections.Generic;
using Checkout.PaymentGateway.Models.ApiModels.Payment;
using Checkout.PaymentGateway.Services;
using Checkout.PaymentGateway.WebApi.Integration.Test.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Checkout.PaymentGateway.WebApi.Integration.Test.Infrastructure
{
    public class ServerFactory : WebApplicationFactory<Startup>
    {
        protected override IWebHostBuilder CreateWebHostBuilder()
        {
            return base.CreateWebHostBuilder()
                .ConfigureAppConfiguration(builder => builder.AddInMemoryCollection(new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("AWS_ACCESS_KEY_ID", "abc"),
                    new KeyValuePair<string, string>("AWS_SECRET_ACCESS_KEY", "def")
                }));
        }
        
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseConfiguration(
                new ConfigurationBuilder().AddInMemoryCollection(new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("AWS_ACCESS_KEY_ID", "abc"),
                    new KeyValuePair<string, string>("AWS_SECRET_ACCESS_KEY", "def")
                }).Build());

            builder.ConfigureServices(services =>
            {
                services.ReplaceServiceType<IPaymentService, TestPaymentService>();
            });
        }
    }
}