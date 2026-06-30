using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apptech.Assessment.EntityFrameworkCore;
using Apptech.Assessment.Products;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.DependencyInjection;

namespace Apptech.Assessment.EntityFrameworkCore.Products;

public class ProductRepository : IProductRepository, ITransientDependency
{
    private readonly AssessmentDbContext _dbContext;

    public ProductRepository(AssessmentDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Dictionary<Guid, ProductOrderReadModel>> GetOrderInfoByIdsAsync(
        IReadOnlyCollection<Guid> productIds
    )
    {
        return await _dbContext.Products
            .Where(product => productIds.Contains(product.Id))
            .Select(product => new ProductOrderReadModel
            {
                Id = product.Id,
                Price = product.Price
            })
            .ToDictionaryAsync(product => product.Id);
    }

    public async Task<bool> TryReserveStockAsync(Guid productId, int quantity)
    {
        var affectedRows = await _dbContext.Products
            .Where(product =>
                product.Id == productId &&
                product.StockQuantity >= quantity
            )
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(
                    product => product.StockQuantity,
                    product => product.StockQuantity - quantity
                )
            );

        return affectedRows == 1;
    }

    public async Task ReleaseStockAsync(Guid productId, int quantity)
    {
        await _dbContext.Products
            .Where(product => product.Id == productId)
            .ExecuteUpdateAsync(setters =>
                setters.SetProperty(
                    product => product.StockQuantity,
                    product => product.StockQuantity + quantity
                )
            );
    }
}