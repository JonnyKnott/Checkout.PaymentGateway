using System;
using Microsoft.Extensions.DependencyInjection;

namespace Checkout.PaymentGateway.WebApi.Client.Payment.DependencyInjection
{
    public static class PaymentClientModule
    {
        public static IServiceCollection AddPaymentServiceClient(
            this IServiceCollection services,
            PaymentClientConfiguration configuration)
        {
            services.AddHttpClient(
                ClientConstants.HttpClientName, 
                client => client.BaseAddress = new Uri(configuration.Endpoint)
                );

            services
                .AddSingleton(configuration)
                .AddSingleton<IPaymentClient, PaymentClient>();

            return services;

        } 
    }
}