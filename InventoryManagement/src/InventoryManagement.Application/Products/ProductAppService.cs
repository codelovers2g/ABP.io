using InventoryManagement.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Caching;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Distributed;

namespace InventoryManagement.Products;

[Authorize]
public class ProductAppService(
    IRepository<Product, Guid> productRepository,
    ProductManager productManager,
    IDistributedCache<ProductCacheItem, Guid> productCache,
    IDistributedEventBus distributedEventBus) : InventoryManagementAppService, IProductAppService
{
    private readonly IRepository<Product, Guid> _productRepository = productRepository;
    private readonly ProductManager _productManager = productManager;
    private readonly IDistributedCache<ProductCacheItem, Guid> _productCache = productCache;
    private readonly IDistributedEventBus _distributedEventBus = distributedEventBus;

    [Authorize(InventoryManagementPermissions.Products.Manage)]
    public async Task<ProductDto> CreateAsync(CreateUpdateProductDto input)
    {
        var product = await _productManager.CreateAsync(
            input.Name,
            input.Code,
            input.Price,
            input.StockCount
        );

        await _productRepository.InsertAsync(product);

        await _distributedEventBus.PublishAsync(new ProductCreatedEto
        {
            Id = product.Id,
            Name = product.Name,
            CreatorId = CurrentUser.Id
        });

        return ObjectMapper.Map<Product, ProductDto>(product);
    }

    [Authorize(InventoryManagementPermissions.Products.Manage)]
    public async Task<ProductDto> UpdateAsync(Guid id, CreateUpdateProductDto input)
    {
        var product = await _productRepository.GetAsync(id);

        await _productManager.UpdateAsync(
            product,
            input.Name,
            input.Code
        );

        product.Price = input.Price;
        product.StockCount = input.StockCount;

        await _productRepository.UpdateAsync(product);

        await _productCache.RemoveAsync(id);

        return ObjectMapper.Map<Product, ProductDto>(product);
    }

    public async Task<ProductDto> GetAsync(Guid id)
    {
        var cacheItem = await _productCache.GetOrAddAsync(
            id,
            async () =>
            {
                var product = await _productRepository.GetAsync(id);
                return ObjectMapper.Map<Product, ProductCacheItem>(product);
            },
            () => new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            }
        );

        return ObjectMapper.Map<ProductCacheItem, ProductDto>(cacheItem!);
    }

    public async Task<PagedResultDto<ProductDto>> GetListAsync(ProductGetListInput input)
    {
        var queryable = await _productRepository.GetQueryableAsync();

        queryable = queryable
            .WhereIf(!input.Filter.IsNullOrWhiteSpace(), x => x.Name.Contains(input.Filter!) || x.Code.Contains(input.Filter!));

        var totalCount = await AsyncExecuter.CountAsync(queryable);

        var products = await AsyncExecuter.ToListAsync(
            queryable
                .OrderBy(input.Sorting ?? nameof(Product.Name))
                .PageBy(input)
        );

        return new PagedResultDto<ProductDto>(
            totalCount,
            ObjectMapper.Map<List<Product>, List<ProductDto>>(products)
        );
    }

    [Authorize(InventoryManagementPermissions.Products.Manage)]
    public async Task DeleteAsync(Guid id)
    {
        await _productRepository.DeleteAsync(id);
        await _productCache.RemoveAsync(id);
    }
}
