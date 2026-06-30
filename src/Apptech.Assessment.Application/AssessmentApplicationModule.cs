using Apptech.Assessment.Features.Orders.Recovery;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Mapperly;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;

namespace Apptech.Assessment;

[DependsOn(
    typeof(AssessmentDomainModule),
    typeof(AssessmentApplicationContractsModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpAccountApplicationModule),
    typeof(AbpTenantManagementApplicationModule),
    typeof(AbpSettingManagementApplicationModule)
    )]
public class AssessmentApplicationModule : AbpModule
{
    public override async Task OnApplicationInitializationAsync(
    ApplicationInitializationContext context)
    {
        await context.AddBackgroundWorkerAsync<ExpiredOrderRecoveryWorker>();
    }
}