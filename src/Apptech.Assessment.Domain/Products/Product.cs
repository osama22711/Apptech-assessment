using System;
using Volo.Abp.Domain.Entities;

namespace Apptech.Assessment.Products;

public class Product : Entity<Guid>
{
    public string Name { get; private set; } = default!;
    public string? Description { get; private set; }
    public decimal Price { get; private set; }
    public int StockQuantity { get; private set; }

    protected Product()
    {
    }

    public Product(
        Guid id,
        string name,
        string? description,
        decimal price,
        int stockQuantity
    ) : base(id)
    {
        Name = name;
        Description = description;
        Price = price;
        StockQuantity = stockQuantity;
    }
}