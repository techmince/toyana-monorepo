using Toyana.Chat.Hubs;

using Toyana.Shared.Extensions; // Observability

var builder = WebApplication.CreateBuilder(args);

// Observability
builder.AddToyanaObservability("chat-api");

// Add SignalR with Redis Backplane
var redisConn = builder.Configuration.GetConnectionString("Valkey") ?? "localhost:6379";
builder.Services.AddSignalR()
    .AddStackExchangeRedis(redisConn, options => {
        options.Configuration.ChannelPrefix = "Toyana.Chat";
    });

var app = builder.Build();

app.UseToyanaObservability();

app.MapGet("/", () => "Toyana.Chat Service");
app.MapHub<EventChatHub>("/chat");

app.Run();
