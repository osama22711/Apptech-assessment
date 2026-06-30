using Volo.Abp.Settings;

namespace Apptech.Assessment.Settings;

public class AssessmentSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(AssessmentSettings.MySetting1));
    }
}
