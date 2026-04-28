using Volo.Abp.Application.Dtos;

namespace InventoryManagement.Products;

public class ProductGetListInput : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
}
