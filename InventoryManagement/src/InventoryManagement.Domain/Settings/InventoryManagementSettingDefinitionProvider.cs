using Volo.Abp.Settings;

namespace InventoryManagement.Settings;

public class InventoryManagementSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(InventoryManagementSettings.MySetting1));
    }
}
