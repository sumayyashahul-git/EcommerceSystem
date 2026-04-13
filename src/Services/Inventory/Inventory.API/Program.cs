using Inventory.Application.Consumers;
using Inventory.Application.Interfaces;
using Inventory.API.Middleware;
using Inventory.Infrastructure.Persistence;
using Inventory.Infrastructure.Repositories;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("InventoryDb")));

builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();

// MassTransit — registers consumer and connects to RabbitMQ
builder.Services.AddMassTransit(x =>
{
    // Register our consumer
    x.AddConsumer<OrderPlacedConsumer>();

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

        // Auto-configure endpoints for all consumers
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

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider
        .GetRequiredService<InventoryDbContext>();
    await db.Database.MigrateAsync();
}

app.Run();