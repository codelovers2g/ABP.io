using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Identity;

namespace InventoryManagement.Data;

public class InventoryManagementIdentityDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IIdentityUserRepository _userRepository;
    private readonly ILookupNormalizer _lookupNormalizer;
    private readonly IdentityUserManager _userManager;
    private readonly IdentityRoleManager _roleManager;

    public InventoryManagementIdentityDataSeedContributor(
        IIdentityUserRepository userRepository,
        ILookupNormalizer lookupNormalizer,
        IdentityUserManager userManager,
        IdentityRoleManager roleManager)
    {
        _userRepository = userRepository;
        _lookupNormalizer = lookupNormalizer;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        if (await _roleManager.FindByNameAsync("admin") == null)
        {
            (await _roleManager.CreateAsync(new IdentityRole(Guid.NewGuid(), "admin"))).CheckErrors();
        }

        var adminUser = await _userRepository.FindByNormalizedUserNameAsync(_lookupNormalizer.NormalizeName("admin"));
        if (adminUser == null)
        {
            adminUser = new IdentityUser(
                Guid.NewGuid(),
                "admin",
                "admin@abp.io"
            );

            (await _userManager.CreateAsync(adminUser, "1q2w3E*")).CheckErrors();
            (await _userManager.AddToRoleAsync(adminUser, "admin")).CheckErrors();
        }
    }
}
