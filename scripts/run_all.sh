#!/bin/bash

# Start ReportingGateway Service
gnome-terminal --title="MIDDLE EARTH - Reporting Gateway" -- bash -c "cd ../SystemMiddleEarth/Kahin.Service.ReportingGateway && dotnet run; exec bash"

# Start Messenger Service
gnome-terminal --title="HOME - Messenger Service" -- bash -c "cd ../SystemHome/GamersWorld.Service.Messenger && dotnet run; exec bash"

# Start Home EventHost
gnome-terminal --title="HOME - Event Consumer Host" -- bash -c "cd ../SystemHome/GamersWorld.EventHost && dotnet run; exec bash"

# Start Home JobHost
gnome-terminal --title="HOME - Scheduled Job Host" -- bash -c "cd ../SystemHome/GamersWorld.JobHost && dotnet run; exec bash"

# Start Home Gateway Service
gnome-terminal --title="HOME - Gateway Service" -- bash -c "cd ../SystemHome/GamersWorld.Service.Gateway && dotnet run; exec bash"

# Start Eval.Api Service
# Docker Service haline getirildiği için gerek kalmadı
# gnome-terminal --title="HAL - Expression Auditor" -- bash -c "cd ../SystemHAL/Eval.AuditApi && dotnet run; exec bash"

# Start Web App
gnome-terminal --title="HOME - Web App" -- bash -c "cd ../SystemHome/GamersWorld.WebApp && dotnet run; exec bash"

# Start Middle Earth System EventHost
gnome-terminal --title="MIDDLE EARTH - Event Consumer Host" -- bash -c "cd ../SystemMiddleEarth/Kahin.EventHost && dotnet run; exec bash"

# Start Asgard System Heimdall
gnome-terminal --title="ASGARD - Heimdall Monitoring UI" -- bash -c "cd ../SystemAsgard/Heimdall && dotnet run; exec bash"