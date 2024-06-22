#!/bin/bash

export AWS_ACCESS_KEY_ID=test
export AWS_SECRET_ACCESS_KEY=test
export AWS_DEFAULT_REGION=eu-west-1

add_secret() {
    environment=$1
    secret_name=$2
    secret_value=$3
    aws --endpoint-url=http://localhost:4566 secretsmanager create-secret \
        --name "$secret_name" \
        --secret-string "$secret_value" \
        --tags Key=Environment,Value=$environment
    echo "Added secret $secret_name for $environment"
}

# Development Secrets
add_secret "Development" "RedisConnectionString" "localhost:6379"
add_secret "Development" "EvalServiceApiAddress" "localhost:5147"
add_secret "Development" "HomeGatewayApiAddress" "localhost:5102"
add_secret "Development" "HomeWebAppAddress" "localhost:5093"
add_secret "Development" "MessengerApiAddress" "localhost:5234"
add_secret "Development" "RabbitMQHostName" "localhost"
add_secret "Development" "RabbitMQUsername" "scothtiger"
add_secret "Development" "RabbitMQPassword" "123456"
add_secret "Development" "RabbitMQPort" "5672"
add_secret "Development" "KahinReportingGatewayApiAddress" "localhost:5218"

# Test Environment Secrets

# Production Environment Secrets