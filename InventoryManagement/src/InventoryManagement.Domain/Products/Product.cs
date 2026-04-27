using System;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;

namespace InventoryManagement.Products;

public class Product : FullAuditedAggregateRoot<Guid>
{
    public string Name { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockCount { get; set; }

    private Product()
    {
    }

    internal Product(
        Guid id,
        string name,
        string code,
        decimal price,
        int stockCount) 
        : base(id)
    {
        SetName(name);
        SetCode(code);
        Price = price;
        StockCount = stockCount;
    }

    internal void SetName(string name)
    {
        Name = Check.NotNullOrWhiteSpace(name, nameof(name), ProductConsts.MaxNameLength);
    }

    internal void SetCode(string code)
    {
        Code = Check.NotNullOrWhiteSpace(code, nameof(code), ProductConsts.MaxCodeLength);
    }
}
