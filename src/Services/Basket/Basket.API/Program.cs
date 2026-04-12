using Basket.API.Middleware;
using Basket.Application.Interfaces;
using Basket.Infrastructure.Repositories;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Redis Connection
// IConnectionMultiplexer is the main Redis connection object
// Registered as Singleton — one connection shared across all requests
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var connectionString = builder.Configuration
        .GetConnectionString("Redis")
        ?? "localhost:6379";

    return ConnectionMultiplexer.Connect(connectionString);
});

// Basket Repository
builder.Services.AddScoped<IBasketRepository, BasketRepository>();

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