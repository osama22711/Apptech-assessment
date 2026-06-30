using System;
using Volo.Abp.Domain.Entities;

namespace Apptech.Assessment.Orders;

public class OrderItem : Entity<Guid>
{
    public Guid OrderId { get; private set; }
    public Guid ProductId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    protected OrderItem()
    {
    }

    public OrderItem(
        Guid id,
        Guid orderId,
        Guid productId,
        int quantity,
        decimal unitPrice
    ) : base(id)
    {
        OrderId = orderId;
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}