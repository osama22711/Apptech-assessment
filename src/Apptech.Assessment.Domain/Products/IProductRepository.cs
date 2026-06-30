using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Apptech.Assessment.Products;

public interface IProductRepository
{
    Task<Dictionary<Guid, ProductOrderReadModel>> GetOrderInfoByIdsAsync(
        IReadOnlyCollection<Guid> productIds
    );

    Task InsertManyAsync(IReadOnlyCollection<Product> products);

    Task<bool> TryReserveStockAsync(Guid productId, int quantity);

    Task ReleaseStockAsync(Guid productId, int quantity);
}