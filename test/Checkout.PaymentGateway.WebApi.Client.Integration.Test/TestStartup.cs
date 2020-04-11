using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;
using Xunit.DependencyInjection;

[assembly: TestFramework(
    "Checkout.PaymentGateway.WebApi.Client.Test.TestStartup",
    "Checkout.PaymentGateway.WebApi.Client.Test")]
namespace Checkout.PaymentGateway.WebApi.Client.Test
{
    public class TestStartup : DependencyInjectionTestFramework
    {
        public TestStartup(IMessageSink messageSink) : base(messageSink)
        {
        }

        protected void ConfigureServices(IServiceCollection services)
        {
            var path = Directory.GetCurrentDirectory();
            
        }
    }
}