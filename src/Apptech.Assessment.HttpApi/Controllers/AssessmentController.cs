using Apptech.Assessment.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace Apptech.Assessment.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class AssessmentController : AbpControllerBase
{
    protected AssessmentController()
    {
        LocalizationResource = typeof(AssessmentResource);
    }
}
