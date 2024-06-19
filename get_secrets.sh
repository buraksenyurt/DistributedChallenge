#!/bin/bash

export AWS_ACCESS_KEY_ID=test
export AWS_SECRET_ACCESS_KEY=test
export AWS_DEFAULT_REGION=eu-west-1

get_secret() {
    secret_name=$1
    secret_value=$(aws --endpoint-url=http://localhost:4566 secretsmanager get-secret-value --secret-id $secret_name --query SecretString --output text)
    echo "$secret_name: $secret_value"
}

get_secret "RedisConnectionString"
get_secret "EvalServiceApiAddress"
get_secret "HomeGatewayApiAddress"
get_secret "MessengerApiAddress"
get_secret "RabbitMQHostName"
get_secret "RabbitMQUsername"
get_secret "RabbitMQPassword"
get_secret "RabbitMQPort"
get_secret "KahinReportingGatewayApiAddres"
