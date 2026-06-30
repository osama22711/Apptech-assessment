using Apptech.Assessment.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace Apptech.Assessment.Permissions;

public class AssessmentPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(AssessmentPermissions.GroupName);

        //Define your own permissions here. Example:
        //myGroup.AddPermission(AssessmentPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<AssessmentResource>(name);
    }
}
