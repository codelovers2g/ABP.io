using InventoryManagement.Samples;
using Xunit;

namespace InventoryManagement.EntityFrameworkCore.Domains;

[Collection(InventoryManagementTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<InventoryManagementEntityFrameworkCoreTestModule>
{

}
