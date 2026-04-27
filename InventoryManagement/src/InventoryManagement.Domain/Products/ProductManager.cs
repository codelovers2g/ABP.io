using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.Domain.Repositories;

namespace InventoryManagement.Products;

public class ProductManager(IRepository<Product, Guid> productRepository) : DomainService
{
    private readonly IRepository<Product, Guid> _productRepository = productRepository;

    public async Task<Product> CreateAsync(
        [NotNull] string name,
        [NotNull] string code,
        decimal price,
        int stockCount)
    {
        Check.NotNullOrWhiteSpace(name, nameof(name));
        Check.NotNullOrWhiteSpace(code, nameof(code));

        var existingProduct = await _productRepository.FirstOrDefaultAsync(p => p.Code == code);
        if (existingProduct != null)
        {
            throw new BusinessException("InventoryManagement:DuplicateProductCode")
                .WithData("code", code);
        }

        return new Product(
            GuidGenerator.Create(),
            name,
            code,
            price,
            stockCount
        );
    }

    public async Task UpdateAsync(
        [NotNull] Product product,
        [NotNull] string name,
        [NotNull] string code)
    {
        Check.NotNull(product, nameof(product));
        Check.NotNullOrWhiteSpace(name, nameof(name));
        Check.NotNullOrWhiteSpace(code, nameof(code));

        var existingProduct = await _productRepository.FirstOrDefaultAsync(p => p.Code == code);
        if (existingProduct != null && existingProduct.Id != product.Id)
        {
            throw new BusinessException("InventoryManagement:DuplicateProductCode")
                .WithData("code", code);
        }

        product.SetName(name);
        product.SetCode(code);
    }
}
