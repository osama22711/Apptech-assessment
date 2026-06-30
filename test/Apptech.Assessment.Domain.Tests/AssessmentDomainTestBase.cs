using Volo.Abp.Modularity;

namespace Apptech.Assessment;

/* Inherit from this class for your domain layer tests. */
public abstract class AssessmentDomainTestBase<TStartupModule> : AssessmentTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
