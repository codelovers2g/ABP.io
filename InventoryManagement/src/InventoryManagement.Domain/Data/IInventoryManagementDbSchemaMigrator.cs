using System.Threading.Tasks;

namespace InventoryManagement.Data;

public interface IInventoryManagementDbSchemaMigrator
{
    Task MigrateAsync();
}
