#!/bin/bash

# Start ReportingGateway
gnome-terminal -- bash -c "cd Kahin.ReportingGateway && dotnet run; exec bash"

# Start Messenger
gnome-terminal -- bash -c "cd GamersWorld.Messenger && dotnet run; exec bash"

# Start EventHost
gnome-terminal -- bash -c "cd GamersWorld.EventHost && dotnet run; exec bash"

# Start Eval.Api
gnome-terminal -- bash -c "cd Eval.Api && dotnet run; exec bash"

# Start Web App
gnome-terminal -- bash -c "cd GamersWorld.WebApp && dotnet run; exec bash"
