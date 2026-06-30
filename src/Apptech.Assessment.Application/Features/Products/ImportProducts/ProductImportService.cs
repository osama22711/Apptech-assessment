using Apptech.Assessment.Features.Products.Autocomplete;
using Apptech.Assessment.Products;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Uow;

namespace Apptech.Assessment.Features.Products.ImportProducts;

public class ProductImportService : ApplicationService
{
    private const int BatchSize = 2000;


    private readonly IProductRepository _productRepository;
    private readonly IProductAutocompleteIndex _autocompleteIndex;

    public ProductImportService(
        IProductRepository productRepository,
        IProductAutocompleteIndex autocompleteIndex)
    {
        _productRepository = productRepository;
        _autocompleteIndex = autocompleteIndex;
    }

    public async Task<ImportProductsResultDto> ImportAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new BusinessException("Invalid CSV file.");
        }

        var importedCount = 0;
        var skippedCount = 0;
        var batch = new List<Product>(BatchSize);

        await using var stream = file.OpenReadStream();
        using var reader = new StreamReader(stream);

        // Expected header:
        // Name,Description,Price,StockQuantity
        await reader.ReadLineAsync();

        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();

            if (string.IsNullOrWhiteSpace(line))
            {
                skippedCount++;
                continue;
            }

            var columns = line.Split(',');

            if (columns.Length < 4)
            {
                skippedCount++;
                continue;
            }

            if (!decimal.TryParse(columns[2], NumberStyles.Number, CultureInfo.InvariantCulture, out var price))
            {
                skippedCount++;
                continue;
            }

            if (!int.TryParse(columns[3], NumberStyles.Integer, CultureInfo.InvariantCulture, out var stockQuantity))
            {
                skippedCount++;
                continue;
            }

            var product = new Product(
                GuidGenerator.Create(),
                columns[0].Trim(),
                columns[1].Trim(),
                price,
                stockQuantity
            );

            batch.Add(product);

            if (batch.Count >= BatchSize)
            {
                await InsertBatchAsync(batch);
                importedCount += batch.Count;
                batch.Clear();
            }
        }

        if (batch.Count > 0)
        {
            await InsertBatchAsync(batch);
            importedCount += batch.Count;
            batch.Clear();
        }

        return new ImportProductsResultDto
        {
            ImportedCount = importedCount,
            SkippedCount = skippedCount
        };
    }

    [UnitOfWork(true)]
    protected virtual async Task InsertBatchAsync(List<Product> products)
    {
        await _productRepository.InsertManyAsync(products);
        await _autocompleteIndex.IndexProductsAsync(products);
    }
}