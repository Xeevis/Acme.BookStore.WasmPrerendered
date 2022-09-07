using Acme.BookStore.WasmPrerendered.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace Acme.BookStore.WasmPrerendered.Permissions;

public class WasmPrerenderedPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(WasmPrerenderedPermissions.GroupName);
        //Define your own permissions here. Example:
        //myGroup.AddPermission(WasmPrerenderedPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<WasmPrerenderedResource>(name);
    }
}
