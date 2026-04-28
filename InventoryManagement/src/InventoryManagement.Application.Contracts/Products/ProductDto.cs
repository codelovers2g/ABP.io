using System;
using Volo.Abp.Application.Dtos;

namespace InventoryManagement.Products;

public class ProductDto : EntityDto<Guid>
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockCount { get; set; }
    public string? ConcurrencyStamp { get; set; }
}
