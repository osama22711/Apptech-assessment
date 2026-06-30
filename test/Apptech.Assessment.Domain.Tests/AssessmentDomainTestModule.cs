using Volo.Abp.Modularity;

namespace Apptech.Assessment;

[DependsOn(
    typeof(AssessmentDomainModule),
    typeof(AssessmentTestBaseModule)
)]
public class AssessmentDomainTestModule : AbpModule
{

}
