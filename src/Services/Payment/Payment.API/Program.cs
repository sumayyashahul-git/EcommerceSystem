using MassTransit;
using Microsoft.EntityFrameworkCore;
using Payment.Application.Consumers;
using Payment.Application.Interfaces;
using Payment.API.Middleware;
using Payment.Infrastructure.Persistence;
using Payment.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PaymentDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("PaymentDb")));

builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

builder.Services.AddMassTransit(x =>
{
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
        .GetRequiredService<PaymentDbContext>();
    await db.Database.MigrateAsync();
}

app.Run();