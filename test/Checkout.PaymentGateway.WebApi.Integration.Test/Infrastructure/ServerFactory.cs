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
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseConfiguration(
                new ConfigurationBuilder().AddInMemoryCollection(new List<KeyValuePair<string, string>>
                {
                }).Build());

            builder.ConfigureServices(services =>
            {
                services.ReplaceServiceType<IPaymentService, TestPaymentService>();
            });
        }
    }
}