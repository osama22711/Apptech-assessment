using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Apptech.Assessment.Data;
using Volo.Abp.DependencyInjection;

namespace Apptech.Assessment.EntityFrameworkCore;

public class EntityFrameworkCoreAssessmentDbSchemaMigrator
    : IAssessmentDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreAssessmentDbSchemaMigrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the AssessmentDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<AssessmentDbContext>()
            .Database
            .MigrateAsync();
    }
}
