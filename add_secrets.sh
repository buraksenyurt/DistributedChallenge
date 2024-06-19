#!/bin/bash

export AWS_ACCESS_KEY_ID=test
export AWS_SECRET_ACCESS_KEY=test
export AWS_DEFAULT_REGION=eu-west-1

add_secret() {
    secret_name=$1
    secret_value=$2
    aws --endpoint-url=http://localhost:4566 secretsmanager create-secret --name $secret_name --secret-string $secret_value
    echo "Added secret $secret_name"
}

add_secret "RedisConnectionString"  "localhost:6379"
add_secret "EvalServiceApiAddress"  "localhost:5147/api"
add_secret "HomeGatewayApiAddress"  "localhost:5102"
add_secret "MessengerApiAddress"    "localhost:5234"