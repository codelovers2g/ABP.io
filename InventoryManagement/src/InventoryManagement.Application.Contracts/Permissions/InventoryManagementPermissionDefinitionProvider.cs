using InventoryManagement.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace InventoryManagement.Permissions;

public class InventoryManagementPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(InventoryManagementPermissions.GroupName, L("Permission:InventoryManagement"));
        
        var productsPermission = myGroup.AddPermission(InventoryManagementPermissions.Products.Default, L("Permission:Products"));
        productsPermission.AddChild(InventoryManagementPermissions.Products.Manage, L("Permission:Manage"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<InventoryManagementResource>(name);
    }
}
