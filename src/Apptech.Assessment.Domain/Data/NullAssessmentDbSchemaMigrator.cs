using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Apptech.Assessment.Data;

/* This is used if database provider does't define
 * IAssessmentDbSchemaMigrator implementation.
 */
public class NullAssessmentDbSchemaMigrator : IAssessmentDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
