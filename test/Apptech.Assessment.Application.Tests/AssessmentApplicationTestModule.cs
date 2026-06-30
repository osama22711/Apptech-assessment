using Volo.Abp.Modularity;

namespace Apptech.Assessment;

[DependsOn(
    typeof(AssessmentApplicationModule),
    typeof(AssessmentDomainTestModule)
)]
public class AssessmentApplicationTestModule : AbpModule
{

}
