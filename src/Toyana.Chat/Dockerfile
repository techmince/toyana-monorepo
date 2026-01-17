FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["src/Toyana.Chat/Toyana.Chat.csproj", "src/Toyana.Chat/"]
COPY ["shared/Toyana.Shared/Toyana.Shared.csproj", "shared/Toyana.Shared/"]
RUN dotnet restore "src/Toyana.Chat/Toyana.Chat.csproj"
COPY . .
WORKDIR "/src/src/Toyana.Chat"
RUN dotnet build "Toyana.Chat.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Toyana.Chat.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
USER $APP_UID
ENTRYPOINT ["dotnet", "Toyana.Chat.dll"]
