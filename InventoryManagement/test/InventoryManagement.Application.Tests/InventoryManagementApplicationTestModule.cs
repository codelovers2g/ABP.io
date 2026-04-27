using Volo.Abp.Modularity;

namespace InventoryManagement;

[DependsOn(
    typeof(InventoryManagementApplicationModule),
    typeof(InventoryManagementDomainTestModule)
)]
public class InventoryManagementApplicationTestModule : AbpModule
{

}
