using Volo.Abp.Modularity;

namespace InventoryManagement;

[DependsOn(
    typeof(InventoryManagementDomainModule),
    typeof(InventoryManagementTestBaseModule)
)]
public class InventoryManagementDomainTestModule : AbpModule
{

}
