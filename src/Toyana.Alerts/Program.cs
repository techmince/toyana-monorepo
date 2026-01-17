using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Toyana.Shared.Extensions; // Observability

var builder = WebApplication.CreateBuilder(args);

// Observability
builder.AddToyanaObservability("alerts-api");
builder.AddToyanaJsonOptions();

// Telegram Bot
// Get Token from Config or Env Var. For now, we expect it in config.
var botToken = builder.Configuration["Telegram:BotToken"];
if (string.IsNullOrEmpty(botToken))
{
    // Warn but don't crash, maybe just disable alerts?
    // Crash is safer to fail fast.
    // throw new Exception("Telegram:BotToken is missing");
    // For development, suppress crash
}

if (!string.IsNullOrEmpty(botToken))
{
    builder.Services.AddHttpClient("telegram_bot_client")
           .AddTypedClient<ITelegramBotClient>((httpClient, sp) =>
           {
               return new TelegramBotClient(botToken, httpClient);
           });
}

var app = builder.Build();

app.UseToyanaObservability();

app.MapPost("/webhook/grafana", async ([FromBody] GrafanaAlertPayload payload, [FromServices] ITelegramBotClient? bot, [FromServices] IConfiguration config, ILogger<Program> logger) =>
{
    if (bot == null) 
    {
        logger.LogWarning("Telegram Bot not configured. Skipping alert.");
        return Results.Ok("Bot not configured");
    }

    var chatId = config["Telegram:ChatId"]; // Target Chat/Group ID
    if (string.IsNullOrEmpty(chatId))
    {
         logger.LogWarning("Telegram:ChatId is missing. Skipping alert.");
         return Results.Ok("ChatId not configured");
    }

    logger.LogInformation("Received Grafana Alert: {Title} - {State}", payload.Title, payload.State);

    // Format Message
    var message = $"🚨 *{payload.Title}*\n" +
                  $"State: {payload.State}\n" +
                  $"Message: {payload.Message}\n";

    if (payload.Alerts != null)
    {
        foreach(var alert in payload.Alerts)
        {
             message += $"\n- *{alert.Labels.Alertname}*: {alert.Annotations.Summary}\n";
        }
    }

    try 
    {
        // Telegram.Bot v22 uses SendMessage
        await bot.SendMessage(chatId, message, parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
        return Results.Ok("Sent");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to send Telegram message");
        return Results.Problem("Failed to send telegram message");
    }
});

app.Run();

// Grafana Payload Models (Simplified)
public class GrafanaAlertPayload
{
    public string? Title { get; set; }
    public string? State { get; set; }
    public string? Message { get; set; }
    public List<GrafanaAlert>? Alerts { get; set; }
}

public class GrafanaAlert
{
    public string? Status { get; set; }
    public GrafanaLabels? Labels { get; set; }
    public GrafanaAnnotations? Annotations { get; set; }
}

public class GrafanaLabels
{
    public string? Alertname { get; set; }
}

public class GrafanaAnnotations
{
    public string? Summary { get; set; }
    public string? Description { get; set; }
}
