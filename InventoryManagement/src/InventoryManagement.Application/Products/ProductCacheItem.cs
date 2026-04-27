using System;
using Volo.Abp.Caching;

namespace InventoryManagement.Products;

[CacheName("InventoryProducts")]
public class ProductCacheItem
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockCount { get; set; }
    public string? ConcurrencyStamp { get; set; }
}
