using System;

namespace Apptech.Assessment.Features.Products.Autocomplete;

public class ProductAutocompleteItemDto
{
    public Guid ProductId { get; set; }

    public string Name { get; set; } = string.Empty;
}