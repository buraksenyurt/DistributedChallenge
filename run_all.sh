#!/bin/bash

# Start ReportingGateway
gnome-terminal --title="Reporting Gateway" -- bash -c "cd SystemMiddleEarth/Kahin.ReportingGateway && dotnet run; exec bash"

# Start Messenger
gnome-terminal --title="Messenger Service" -- bash -c "cd SystemHome/GamersWorld.Messenger && dotnet run; exec bash"

# Start EventHost
gnome-terminal --title="Event Consumer Host" -- bash -c "cd SystemHome/GamersWorld.EventHost && dotnet run; exec bash"

# Start Eval.Api
gnome-terminal --title="Expression Auditor" -- bash -c "cd SystemHAL/Eval.AuditApi && dotnet run; exec bash"

# Start Web App
gnome-terminal --title="Web Application" -- bash -c "cd SystemHome/GamersWorld.WebApp && dotnet run; exec bash"
