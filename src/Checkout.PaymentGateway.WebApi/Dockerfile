FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS source
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY Checkout.PaymentGateway.sln ./
COPY src/Checkout.PaymentGateway.WebApi/Checkout.PaymentGateway.WebApi.csproj src/Checkout.PaymentGateway.WebApi/
COPY src/Checkout.PaymentGateway.Services/Checkout.PaymentGateway.Services.csproj src/Checkout.PaymentGateway.Services/
COPY src/Checkout.PaymentGateway.Data/Checkout.PaymentGateway.Data.csproj src/Checkout.PaymentGateway.Data/
COPY src/Checkout.PaymentGateway.Core/Checkout.PaymentGateway.Core.csproj src/Checkout.PaymentGateway.Core/
COPY src/Checkout.PaymentGateway.Models/Checkout.PaymentGateway.Models.csproj src/Checkout.PaymentGateway.Models/
COPY test/Checkout.PaymentGateway.WebApi.Integration.Test/Checkout.PaymentGateway.WebApi.Integration.Test.csproj test/Checkout.PaymentGateway.WebApi.Integration.Test/
COPY test/Checkout.PaymentGateway.Services.Test/Checkout.PaymentGateway.Services.Test.csproj test/Checkout.PaymentGateway.Services.Test/
COPY test/Checkout.PaymentGateway.Data.Test/Checkout.PaymentGateway.Data.Test.csproj test/Checkout.PaymentGateway.Data.Test/
RUN dotnet restore

WORKDIR /src
COPY . .

FROM source as publish
WORKDIR /src/src/Checkout.PaymentGateway.WebApi
RUN dotnet build -c Release -o /app
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 as base
WORKDIR /ap
COPY --from=publish /app .
EXPOSE 80
ENTRYPOINT ["dotnet", "Checkout.PaymentGateway.WebApi.dll"]