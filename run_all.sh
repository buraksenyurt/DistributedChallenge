#!/bin/bash

# Start ReportingGateway
gnome-terminal --title="MIDDLE EARTH - Reporting Gateway" -- bash -c "cd SystemMiddleEarth/Kahin.ReportingGateway && dotnet run; exec bash"

# Start Messenger
gnome-terminal --title="HOME - Messenger Service" -- bash -c "cd SystemHome/GamersWorld.Messenger && dotnet run; exec bash"

# Start Home EventHost
gnome-terminal --title="HOME - Event Consumer Host" -- bash -c "cd SystemHome/GamersWorld.EventHost && dotnet run; exec bash"

# Start Home Gateway
gnome-terminal --title="HOME - Gateway Service" -- bash -c "cd SystemHome/GamersWorld.Gateway && dotnet run; exec bash"

# Start Eval.Api
gnome-terminal --title="HAL - Expression Auditor" -- bash -c "cd SystemHAL/Eval.AuditApi && dotnet run; exec bash"

# Start Web App
gnome-terminal --title="HOME - Web App" -- bash -c "cd SystemHome/GamersWorld.WebApp && dotnet run; exec bash"

# Start Middle Earth System EventHost
gnome-terminal --title="MIDDLE EARTH - Event Consumer Host" -- bash -c "cd SystemMiddleEarth/Kahin.EventHost && dotnet run; exec bash"