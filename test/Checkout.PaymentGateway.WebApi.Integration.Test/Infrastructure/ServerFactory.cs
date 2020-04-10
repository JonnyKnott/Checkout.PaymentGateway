using System.Collections.Generic;
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

            builder.ConfigureServices(services => { });
        }
    }
}