#!/bin/bash

export AWS_ACCESS_KEY_ID=test
export AWS_SECRET_ACCESS_KEY=test
export AWS_DEFAULT_REGION=eu-west-1

get_secret() {
    environment=$1
    secret_name=$2
    secret_arn=$(aws --endpoint-url=http://localhost:4566 secretsmanager list-secrets \
        --filter Key=tag-key,Values=Environment Key=tag-value,Values=$environment \
        --query "SecretList[?Name=='$secret_name'].ARN" --output text)
    
    if [ -n "$secret_arn" ]; then
        secret_value=$(aws --endpoint-url=http://localhost:4566 secretsmanager get-secret-value --secret-id $secret_arn --query SecretString --output text)
        echo "$environment:$secret_name -> $secret_value"
    else
        echo "$environment:$secret_name not found"
    fi
}

environment="Development"

get_secret $environment "RedisConnectionString"
get_secret $environment "RabbitMQHostName"
get_secret $environment "RabbitMQUsername"
get_secret $environment "RabbitMQPassword"
get_secret $environment "RabbitMQPort"
get_secret $environment "GamersWorldDbConnStr"
get_secret $environment "HomeWebAppHubAddress"
get_secret $environment "FtpServerAddress"
get_secret $environment "FtpUsername"
get_secret $environment "FtpPassword"
get_secret $environment "ElasticsearchAddress"
get_secret $environment "JwtKey"
get_secret $environment "JwtIssuer"
get_secret $environment "JwtAudience"
get_secret $environment "JwtExpiryMinutes"

# environment="Test"

# environment="Production"