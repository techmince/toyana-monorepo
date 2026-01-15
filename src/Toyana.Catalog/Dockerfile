FROM mcr.microsoft.com/dotnet/aspnet:10.0-alpine AS base
WORKDIR /app
EXPOSE 8080
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
RUN apk add --no-cache icu-libs krb5-libs

FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine AS build
WORKDIR /src
COPY ["src/Toyana.Catalog/Toyana.Catalog.csproj", "src/Toyana.Catalog/"]
COPY ["shared/Toyana.Contracts/Toyana.Contracts.csproj", "shared/Toyana.Contracts/"]
COPY ["shared/Toyana.Shared/Toyana.Shared.csproj", "shared/Toyana.Shared/"]
RUN dotnet restore "src/Toyana.Catalog/Toyana.Catalog.csproj"
COPY . .
WORKDIR "/src/src/Toyana.Catalog"
RUN dotnet build "Toyana.Catalog.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Toyana.Catalog.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "Toyana.Catalog.dll"]
