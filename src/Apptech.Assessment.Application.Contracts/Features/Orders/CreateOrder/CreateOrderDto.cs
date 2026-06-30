using System.Collections.Generic;

namespace Apptech.Assessment.Features.Orders.CreateOrder;

public class CreateOrderDto
{
    public List<CreateOrderItemDto> Items { get; set; } = new();
}