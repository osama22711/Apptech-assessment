using System;

namespace Apptech.Assessment.Features.Orders.CreateOrder;

public class CreateOrderItemDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}