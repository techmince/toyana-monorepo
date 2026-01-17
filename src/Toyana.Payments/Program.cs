using JasperFx;
using Marten;
using Toyana.Payments.Interfaces;
using Toyana.Payments.Services;
using Toyana.Shared.Extensions;
using Wolverine;
using Wolverine.Marten;
using Wolverine.RabbitMQ; // Observability

var builder = Host.CreateApplicationBuilder(args);

// Observability
builder.AddToyanaObservability("payments-worker");
builder.AddToyanaJsonOptions();

// DI
builder.Services.AddSingleton<IFeeStrategy, StandardFeeStrategy>();
builder.Services.AddSingleton<FeeCalculator>();
builder.Services.AddSingleton<IDebitCardProvider, MockDebitCardProvider>();
builder.Services.AddSingleton<IBankAccountProvider, MockBankAccountProvider>();

// Marten (Event Store)
builder.Services.AddMarten(opts =>
{
    var conn = builder.Configuration.GetConnectionString("Postgres");
    opts.Connection(conn);
    opts.DatabaseSchemaName = "payments";
    opts.AutoCreateSchemaObjects = AutoCreate.All;
})
.IntegrateWithWolverine();

// Wolverine
builder.Services.AddWolverine(opts =>
{
    var rabbitConn = builder.Configuration.GetConnectionString("RabbitMq");
    opts.UseRabbitMq(new Uri(rabbitConn))
        .AutoProvision();
    
    // Listen to "toyana.payments" queue? 
    // Or just let Wolverine handle command routing.
});

var host = builder.Build();
host.Run();
