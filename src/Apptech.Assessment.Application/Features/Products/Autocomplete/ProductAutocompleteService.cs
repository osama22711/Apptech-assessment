using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Apptech.Assessment.Features.Products.Autocomplete;

public class ProductAutocompleteService : ApplicationService
{
    private readonly IProductAutocompleteIndex _autocompleteIndex;

    public ProductAutocompleteService(IProductAutocompleteIndex autocompleteIndex)
    {
        _autocompleteIndex = autocompleteIndex;
    }

    [IgnoreAntiforgeryToken]
    public async Task<List<ProductAutocompleteItemDto>> GetAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Trim().Length < 3)
        {
            return new List<ProductAutocompleteItemDto>();
        }

        return await _autocompleteIndex.SearchAsync(query, limit: 10);
    }
}