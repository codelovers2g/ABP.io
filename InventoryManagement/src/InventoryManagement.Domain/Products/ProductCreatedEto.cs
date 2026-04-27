using System;

namespace InventoryManagement.Products;

public class ProductCreatedEto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid? CreatorId { get; set; }
}
