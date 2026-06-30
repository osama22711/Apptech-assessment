using Apptech.Assessment.Localization;
using Volo.Abp.Application.Services;

namespace Apptech.Assessment;

/* Inherit your application services from this class.
 */
public abstract class AssessmentAppService : ApplicationService
{
    protected AssessmentAppService()
    {
        LocalizationResource = typeof(AssessmentResource);
    }
}
