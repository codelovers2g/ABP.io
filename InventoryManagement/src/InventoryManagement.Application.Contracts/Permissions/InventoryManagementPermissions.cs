namespace InventoryManagement.Permissions;

public static class InventoryManagementPermissions
{
    public const string GroupName = "Inventory";

    public static class Products
    {
        public const string Default = GroupName + ".Products";
        public const string Manage = Default + ".Manage";
    }
}
