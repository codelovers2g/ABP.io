using InventoryManagement.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace InventoryManagement.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(InventoryManagementEntityFrameworkCoreModule),
    typeof(InventoryManagementApplicationContractsModule)
    )]
public class InventoryManagementDbMigratorModule : AbpModule
{
}
