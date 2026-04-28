using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace InventoryManagement.Data;

/* This is used if database provider does't define
 * IInventoryManagementDbSchemaMigrator implementation.
 */
public class NullInventoryManagementDbSchemaMigrator : IInventoryManagementDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
