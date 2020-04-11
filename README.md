# Checkout.PaymentGateway

## Application Overview

The Checkout.PaymentGateway project is designed to allow Merchants to send card payments to their acquiring bank and then check payment status after the fact.

The API provides two endpoints:

- Make Payment
    - Endpoint
    ```http request
    POST - {baseUrl}/api/v1/payment
    ```
    - Request Content
    ```text
    content-type: application/json
    ``` 
    ```json
    {
        "CardNumber": "CardNumber",
        "Cvv": "Cvv",
        "Amount":100.00,
        "ExpiryDate":"MM/yy",
        "Currency":"GBP"
    }
    ```
    - Response
    ```json
    {
        "responseValue": {
            "paymentIdentifier": "70477d7e-41a0-4018-aca7-3ae2399783af",
            "status": "Complete"
        },
        "validationErrors": [
            {
                "fieldName": "FieldName",
                "error": "Reason"
            }
        ] 
    }
    ```

- Get Payment Result
    - Endpoint
    ```http request
    GET - {baseUrl}/api/v1/payment/{paymentIdentifier}
    ```
    - Response
    ```json
    {
        "paymentIdentifier": "{{Guid}}",
        "cardNumber": "CardNumber",
        "amount": "Amount",
        "timestamp": "yyyy-MM-ddTHH:mm:ss",
        "status": "Complete"
    }
    ```

## Pre-requisites

- The application uses .NET Core 3.1.
- The application requires access to a dynamodb table to persist payment data. This can be set up below using Docker.
- It is recommended to build and run the application using the docker-compose file.

## Running the application

### Configuration

Configuration is stored in the launchSettings.json file found at `src/Checkout.PaymentGateway.WebApi/Properties/launchSettings.json`, or passed as environment variables in `docker-compose.yml`.

The configuration values are currently set up for use with the exposed docker DynamoDb local instance.
 
## Running application in IDE
 
 - If you want to run the application via your IDE run the `docker-compose.test.yml` file as follows:
 
     ```shell
    docker-compose -f docker-compose.test.yml up
    ``` 
 
 - The application can now be built and run via your IDE, running `Checkout.PaymentGateway.WebApi` project.
    - The WebApi project will create the required table if it does not exist.
 
## Running application with Docker
 
 - In order to run the application using Docker, navigate to the repository root in a terminal.
 
 - Build the WebApi image
 
     ```shell
    docker-compose build webapi
    ```
 - Bring up the docker container
    ```shell
    docker-compose up
    ``` 
 
 - When running in the container the API endpoint is exposed as `http://localhost:9002/` 
 
 ## Running Tests
 
 - The Some of the tests require a local running DynamoDb service. Please ensure you have the docker test containers running by using the following command:
    ```shell
    docker-compose -f docker-compose.test.yml up
    ``` 
 
 ## Postman Collection
 
 The postman collection and environment provided in the repository has example calls for the API.
 
 The collection can be found at `postman/Checkout.PaymentGateway.postman_collection.json` and the environment at `postman/Checkout.PaymentGateway.postman_environment.json`