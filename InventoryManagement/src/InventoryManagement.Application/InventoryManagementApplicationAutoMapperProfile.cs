using AutoMapper;
using InventoryManagement.Products;

namespace InventoryManagement;

public class InventoryManagementApplicationAutoMapperProfile : Profile
{
    public InventoryManagementApplicationAutoMapperProfile()
    {
        CreateMap<Product, ProductDto>();
        CreateMap<Product, ProductCacheItem>().ReverseMap();
        CreateMap<ProductCacheItem, ProductDto>();
        
        CreateMap<CreateUpdateProductDto, Product>()
            .ForMember(x => x.ConcurrencyStamp, opt => opt.Condition(src => !string.IsNullOrEmpty(src.ConcurrencyStamp)));
    }
}
