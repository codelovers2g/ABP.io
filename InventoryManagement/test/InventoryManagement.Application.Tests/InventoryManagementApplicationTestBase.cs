using Volo.Abp.Modularity;

namespace InventoryManagement;

public abstract class InventoryManagementApplicationTestBase<TStartupModule> : InventoryManagementTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
