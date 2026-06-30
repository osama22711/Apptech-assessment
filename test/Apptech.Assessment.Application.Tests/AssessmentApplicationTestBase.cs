using Volo.Abp.Modularity;

namespace Apptech.Assessment;

public abstract class AssessmentApplicationTestBase<TStartupModule> : AssessmentTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
