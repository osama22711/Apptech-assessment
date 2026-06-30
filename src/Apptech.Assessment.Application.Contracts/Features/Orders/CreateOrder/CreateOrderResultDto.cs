using System;

namespace Apptech.Assessment.Features.Orders.CreateOrder;

public class CreateOrderResultDto
{
    public bool Success { get; set; }
    public Guid? OrderId { get; set; }
    public string? Error { get; set; }
}