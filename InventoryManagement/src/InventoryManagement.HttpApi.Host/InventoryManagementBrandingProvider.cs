using Microsoft.Extensions.Localization;
using InventoryManagement.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace InventoryManagement;

[Dependency(ReplaceServices = true)]
public class InventoryManagementBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<InventoryManagementResource> _localizer;

    public InventoryManagementBrandingProvider(IStringLocalizer<InventoryManagementResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
