using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;

namespace Apptech.Assessment.Orders;

public class Order : AuditedAggregateRoot<Guid>
{
    public OrderStatus Status { get; private set; }
    public DateTime ExpiresAtUtc { get; private set; }

    public ICollection<OrderItem> Items { get; private set; } = new List<OrderItem>();

    protected Order()
    {
    }

    public Order(Guid id, DateTime expiresAtUtc)
        : base(id)
    {
        Status = OrderStatus.PendingPayment;
        ExpiresAtUtc = expiresAtUtc;
    }

    public void AddItem(Guid productId, int quantity, decimal unitPrice)
    {
        Items.Add(new OrderItem(Guid.NewGuid(), Id, productId, quantity, unitPrice));
    }

    public void MarkPaid()
    {
        if (Status != OrderStatus.PendingPayment)
        {
            throw new InvalidOperationException("Only pending payment orders can be paid.");
        }

        Status = OrderStatus.Paid;
    }

    public void MarkExpired()
    {
        if (Status == OrderStatus.PendingPayment)
        {
            Status = OrderStatus.Expired;
        }
    }
}