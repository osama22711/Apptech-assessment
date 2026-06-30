using System;
using System.Linq;
using System.Threading.Tasks;
using Apptech.Assessment.Features.Orders.CreateOrder;
using Apptech.Assessment.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Guids;
using Xunit;

namespace Apptech.Assessment.EntityFrameworkCore.IntegrationTests.Orders;

public class OrderServiceIntegrationTests
    : AssessmentTestBase<AssessmentEntityFrameworkCoreTestModule>
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly AssessmentDbContext _dbContext;
    private readonly IGuidGenerator _guidGenerator;

    public OrderServiceIntegrationTests()
    {
        _scopeFactory = GetRequiredService<IServiceScopeFactory>();
        _dbContext = GetRequiredService<AssessmentDbContext>();
        _guidGenerator = GetRequiredService<IGuidGenerator>();
    }

    [Fact]
    public async Task CreateAsync_WhenTwoConcurrentOrdersRequestLastRemainingItem_ShouldCreateExactlyOneOrderAndRejectOne()
    {
        // Arrange
        var productId = _guidGenerator.Create();

        var product = new Product(
            productId,
            "Flash Sale Product",
            "Only one item available",
            100,
            1
        );

        await _dbContext.Products.AddAsync(product);
        await _dbContext.SaveChangesAsync();

        var request1 = CreateOrderRequest(productId);
        var request2 = CreateOrderRequest(productId);

        // Act
        var task1 = TryCreateOrderInNewScopeAsync(request1);
        var task2 = TryCreateOrderInNewScopeAsync(request2);

        var results = await Task.WhenAll(task1, task2);

        // Assert
        results.Count(success => success).ShouldBe(1);
        results.Count(success => !success).ShouldBe(1);

        var updatedProduct = await _dbContext.Products
            .AsNoTracking()
            .FirstAsync(product => product.Id == productId);

        updatedProduct.StockQuantity.ShouldBe(0);
    }

    private static CreateOrderDto CreateOrderRequest(Guid productId)
    {
        return new CreateOrderDto
        {
            Items =
            {
                new CreateOrderItemDto
                {
                    ProductId = productId,
                    Quantity = 1
                }
            }
        };
    }

    private async Task<bool> TryCreateOrderInNewScopeAsync(CreateOrderDto request)
    {
        using var scope = _scopeFactory.CreateScope();

        var sut = scope.ServiceProvider.GetRequiredService<OrderService>();

        try
        {
            var result = await sut.CreateAsync(request);
            return result.Success;
        }
        catch (BusinessException)
        {
            return false;
        }
    }
}