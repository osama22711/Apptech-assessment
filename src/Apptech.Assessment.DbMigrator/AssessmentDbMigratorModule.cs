using Apptech.Assessment.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Apptech.Assessment.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(AssessmentEntityFrameworkCoreModule),
    typeof(AssessmentApplicationContractsModule)
)]
public class AssessmentDbMigratorModule : AbpModule
{
}
