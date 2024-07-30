#!/bin/bash

export AWS_ACCESS_KEY_ID=test
export AWS_SECRET_ACCESS_KEY=test
export AWS_DEFAULT_REGION=eu-west-1

add_or_update_secret() {
    environment=$1
    secret_name=$2
    secret_value=$3
    
    existing_secret=$(aws --endpoint-url=http://localhost:4566 secretsmanager describe-secret --secret-id "$secret_name" 2>/dev/null)
    
    if [ -z "$existing_secret" ]; then
        aws --endpoint-url=http://localhost:4566 secretsmanager create-secret \
            --name "$secret_name" \
            --secret-string "$secret_value" \
            --tags Key=Environment,Value=$environment
        echo "Added secret $secret_name for $environment"
    else
        aws --endpoint-url=http://localhost:4566 secretsmanager update-secret \
            --secret-id "$secret_name" \
            --secret-string "$secret_value"
        echo "Updated secret $secret_name for $environment"
    fi
}

# Development Secrets
add_or_update_secret "Development" "RedisConnectionString" "localhost:6379"
add_or_update_secret "Development" "RabbitMQHostName" "localhost"
add_or_update_secret "Development" "RabbitMQUsername" "scothtiger"
add_or_update_secret "Development" "RabbitMQPassword" "123456"
add_or_update_secret "Development" "RabbitMQPort" "5672"
add_or_update_secret "Development" "RabbitAmqpConnectionString" "amqp://scothtiger:123456@localhost:5672/"
add_or_update_secret "Development" "GamersWorldDbConnStr" "Host=localhost;Port=5432;Username=johndoe;Password=somew0rds;Database=GamersWorldDb"
add_or_update_secret "Development" "HomeWebAppHubAddress" "localhost:5093/notifyHub"
add_or_update_secret "Development" "FtpServerAddress" "localhost"
add_or_update_secret "Development" "FtpUsername" "userone"
add_or_update_secret "Development" "FtpPassword" "123"
add_or_update_secret "Development" "ElasticsearchAddress" "localhost:9200"
add_or_update_secret "Development" "JwtKey" "1ts@beauitfullyday1s8ntperf@c90t1903"
add_or_update_secret "Development" "JwtIssuer" "BuraksGamersWorld"
add_or_update_secret "Development" "JwtAudience" "GamersWorldIdentityAPI"
add_or_update_secret "Development" "JwtExpiryMinutes" "60"