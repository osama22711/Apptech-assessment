using Microsoft.Extensions.Localization;
using Apptech.Assessment.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace Apptech.Assessment;

[Dependency(ReplaceServices = true)]
public class AssessmentBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<AssessmentResource> _localizer;

    public AssessmentBrandingProvider(IStringLocalizer<AssessmentResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
