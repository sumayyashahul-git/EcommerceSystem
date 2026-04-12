using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.Json;
using Basket.Application.Interfaces;
using Basket.Domain.Models;
using StackExchange.Redis;

namespace Basket.Infrastructure.Repositories;

/// <summary>
/// Redis implementation of IBasketRepository.
/// All cart data stored in Redis as JSON strings.
///
/// Redis Key pattern: "basket:{userId}"
/// Example key:       "basket:ab1a6aa5-a41f-4dd7-a609"
/// Example value:     "{"userId":"ab1a...","items":[...]}"
/// </summary>
public class BasketRepository : IBasketRepository
{
    private readonly IDatabase _redisDb;

    // Cart expires after 24 hours of inactivity
    private readonly TimeSpan _cartExpiry = TimeSpan.FromHours(24);

    public BasketRepository(IConnectionMultiplexer redis)
    {
        // Get the Redis database instance
        _redisDb = redis.GetDatabase();
    }

    public async Task<BasketCart?> GetBasketAsync(string userId)
    {
        // Build the Redis key
        var key = GetKey(userId);

        // Get JSON string from Redis
        var data = await _redisDb.StringGetAsync(key);

        // If key doesn't exist → return null
        if (data.IsNullOrEmpty)
            return null;

        // Deserialize JSON back to BasketCart object
        return JsonSerializer.Deserialize<BasketCart>(data!);
    }

    public async Task<BasketCart> UpdateBasketAsync(BasketCart basket)
    {
        var key = GetKey(basket.UserId);

        // Serialize cart to JSON string
        var json = JsonSerializer.Serialize(basket);

        // Store in Redis with 24 hour expiry
        // After 24 hours → Redis auto-deletes this key
        await _redisDb.StringSetAsync(key, json, _cartExpiry);

        return basket;
    }

    public async Task DeleteBasketAsync(string userId)
    {
        var key = GetKey(userId);

        // Delete the key from Redis
        await _redisDb.KeyDeleteAsync(key);
    }

    public async Task<bool> BasketExistsAsync(string userId)
    {
        var key = GetKey(userId);

        // Check if key exists in Redis
        return await _redisDb.KeyExistsAsync(key);
    }

    // Helper — builds consistent Redis key
    // Always "basket:{userId}" format
    private static string GetKey(string userId)
        => $"basket:{userId}";
}