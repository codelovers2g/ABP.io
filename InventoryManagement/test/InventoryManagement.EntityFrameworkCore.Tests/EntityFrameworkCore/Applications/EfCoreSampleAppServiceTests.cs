using InventoryManagement.Samples;
using Xunit;

namespace InventoryManagement.EntityFrameworkCore.Applications;

[Collection(InventoryManagementTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<InventoryManagementEntityFrameworkCoreTestModule>
{

}
