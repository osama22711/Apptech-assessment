using Apptech.Assessment.Orders;
using Apptech.Assessment.Products;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Timing;
using Volo.Abp.Uow;

namespace Apptech.Assessment.Features.Orders.CreateOrder;

public class OrderService : ApplicationService
{
    private readonly IProductRepository _productRepository;
    private readonly IRepository<Order, Guid> _orderRepository;
    private readonly IClock _clock;

    public OrderService(
        IProductRepository productRepository,
        IRepository<Order, Guid> orderRepository,
        IClock clock)
    {
        _productRepository = productRepository;
        _orderRepository = orderRepository;
        _clock = clock;
    }

    [IgnoreAntiforgeryToken]
    [UnitOfWork(isTransactional: true)]
    public async Task<CreateOrderResultDto> CreateAsync(CreateOrderDto input)
    {
        if (input.Items == null || input.Items.Count == 0)
        {
            throw new BusinessException("Basket is empty.");
        }

        if (input.Items.Any(item => item.Quantity <= 0))
        {
            throw new BusinessException("Quantity must be greater than zero.");
        }

        var normalizedItems = input.Items
            .GroupBy(item => item.ProductId)
            .Select(group => new CreateOrderItemDto
            {
                ProductId = group.Key,
                Quantity = group.Sum(item => item.Quantity)
            })
            .OrderBy(item => item.ProductId)
            .ToList();

        var productIds = normalizedItems
            .Select(item => item.ProductId)
            .ToList();

        var products = await _productRepository.GetOrderInfoByIdsAsync(productIds);

        if (products.Count != normalizedItems.Count)
        {
            throw new BusinessException("One or more products do not exist.");
        }

        foreach (var item in normalizedItems)
        {
            var reserved = await _productRepository.TryReserveStockAsync(
                item.ProductId,
                item.Quantity
            );

            if (!reserved)
            {
                throw new BusinessException("Insufficient stock.");
            }
        }

        var order = new Order(
            GuidGenerator.Create(),
            _clock.Now.ToUniversalTime().AddMinutes(15)
        );

        foreach (var item in normalizedItems)
        {
            var product = products[item.ProductId];

            order.AddItem(
                product.Id,
                item.Quantity,
                product.Price
            );
        }

        await _orderRepository.InsertAsync(order, autoSave: true);

        return new CreateOrderResultDto
        {
            Success = true,
            OrderId = order.Id
        };
    }
}