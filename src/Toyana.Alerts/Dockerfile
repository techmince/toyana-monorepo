FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/Toyana.Alerts/Toyana.Alerts.csproj", "src/Toyana.Alerts/"]
COPY ["shared/Toyana.Shared/Toyana.Shared.csproj", "shared/Toyana.Shared/"]
RUN dotnet restore "src/Toyana.Alerts/Toyana.Alerts.csproj"
COPY . .
WORKDIR "/src/src/Toyana.Alerts"
RUN dotnet build "Toyana.Alerts.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Toyana.Alerts.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "Toyana.Alerts.dll"]
