FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["nuget.config", "."]
COPY ["Eval.AuditApi/Eval.AuditApi.csproj", "Eval.AuditApi/"]
RUN dotnet restore "./Eval.AuditApi/Eval.AuditApi.csproj"
COPY . .
WORKDIR "/src/Eval.AuditApi"
RUN dotnet build "./Eval.AuditApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Eval.AuditApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Eval.AuditApi.dll"]
