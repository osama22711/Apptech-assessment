using System.Collections.Generic;
using System.Threading.Tasks;
using Apptech.Assessment.Products;

namespace Apptech.Assessment.Features.Products.Autocomplete;

public interface IProductAutocompleteIndex
{
    Task IndexProductsAsync(IEnumerable<Product> products);

    Task<List<ProductAutocompleteItemDto>> SearchAsync(string query, int limit = 10);
}