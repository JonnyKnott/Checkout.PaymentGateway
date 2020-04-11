using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Checkout.PaymentGateway.WebApi.Services.JsonConverters
{
    public class PaymentAmountJsonConverter : JsonConverter<decimal>
    {
        public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.GetDecimal();
        }

        public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options)
        {
            var rounded = Math.Round(value, 2);
            
            writer.WriteStringValue(rounded.ToString("N"));
        }
    }
}