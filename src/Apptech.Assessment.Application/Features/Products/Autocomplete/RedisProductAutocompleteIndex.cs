using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Apptech.Assessment.Products;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using Volo.Abp.DependencyInjection;

namespace Apptech.Assessment.Features.Products.Autocomplete;

public class RedisProductAutocompleteIndex :
    IProductAutocompleteIndex,
    ISingletonDependency
{
    private readonly Lazy<ConnectionMultiplexer> _connection;

    public RedisProductAutocompleteIndex(IConfiguration configuration)
    {
        var redisConfiguration =
            configuration["Redis:Configuration"] ??
            "localhost:6379";

        _connection = new Lazy<ConnectionMultiplexer>(() =>
            ConnectionMultiplexer.Connect(redisConfiguration)
        );
    }

    public async Task IndexProductsAsync(IEnumerable<Product> products)
    {
        var database = _connection.Value.GetDatabase();
        var batch = database.CreateBatch();

        var tasks = new List<Task>();

        foreach (var product in products)
        {
            var normalizedName = Normalize(product.Name);

            if (normalizedName.Length < 3)
            {
                continue;
            }

            var maxPrefixLength = Math.Min(normalizedName.Length, 30);

            var value = JsonSerializer.Serialize(new ProductAutocompleteItemDto
            {
                ProductId = product.Id,
                Name = product.Name
            });

            for (var length = 3; length <= maxPrefixLength; length++)
            {
                var prefix = normalizedName[..length];
                var key = BuildKey(prefix);

                tasks.Add(batch.SortedSetAddAsync(key, value, score: 1));
            }
        }

        batch.Execute();

        await Task.WhenAll(tasks);
    }

    public async Task<List<ProductAutocompleteItemDto>> SearchAsync(
    string query,
    int limit = 10
)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return new List<ProductAutocompleteItemDto>();
        }

        var normalizedQuery = Normalize(query);

        if (normalizedQuery.Length < 3)
        {
            return new List<ProductAutocompleteItemDto>();
        }

        var database = _connection.Value.GetDatabase();

        var values = await database.SortedSetRangeByRankAsync(
            BuildKey(normalizedQuery),
            0,
            limit - 1,
            StackExchange.Redis.Order.Descending
        );

        var results = new List<ProductAutocompleteItemDto>();

        foreach (var value in values)
        {
            if (!value.HasValue)
            {
                continue;
            }

            var item = JsonSerializer.Deserialize<ProductAutocompleteItemDto>(
                value.ToString()
            );

            if (item != null)
            {
                results.Add(item);
            }
        }

        return results;
    }


    private static string Normalize(string value)
    {
        return value.Trim().ToLowerInvariant();
    }

    private static string BuildKey(string prefix)
    {
        return $"autocomplete:products:{prefix}";
    }
}