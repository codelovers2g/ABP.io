using System;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Domain.Entities;

namespace InventoryManagement.Products;

public class CreateUpdateProductDto : IHasConcurrencyStamp
{
    [Required]
    [StringLength(ProductConsts.MaxNameLength)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(ProductConsts.MaxCodeLength)]
    public string Code { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int StockCount { get; set; }

    public string? ConcurrencyStamp { get; set; }
}
