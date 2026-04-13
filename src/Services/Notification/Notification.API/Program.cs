using MassTransit;
using Notification.Application.Consumers;
using Notification.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// MassTransit — registers ALL consumers
builder.Services.AddMassTransit(x =>
{
    // Register all notification consumers
    x.AddConsumer<OrderPlacedConsumer>();
    x.AddConsumer<PaymentProcessedConsumer>();
    x.AddConsumer<StockReservedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(
            builder.Configuration["RabbitMQ:Host"] ?? "localhost",
            builder.Configuration["RabbitMQ:VirtualHost"] ?? "/",
            h =>
            {
                h.Username(builder.Configuration["RabbitMQ:Username"]
                    ?? "guest");
                h.Password(builder.Configuration["RabbitMQ:Password"]
                    ?? "guest");
            });

        cfg.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();