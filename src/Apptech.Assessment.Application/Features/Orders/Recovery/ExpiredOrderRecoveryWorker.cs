using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Volo.Abp.BackgroundWorkers;
using Volo.Abp.Threading;

namespace Apptech.Assessment.Features.Orders.Recovery;

public class ExpiredOrderRecoveryWorker : AsyncPeriodicBackgroundWorkerBase
{
    public ExpiredOrderRecoveryWorker(
        AbpAsyncTimer timer,
        IServiceScopeFactory serviceScopeFactory
    ) : base(timer, serviceScopeFactory)
    {
        Timer.Period = 60_000; // 1 minute
    }

    protected override async Task DoWorkAsync(
        PeriodicBackgroundWorkerContext workerContext)
    {
        var recoveryService = workerContext
            .ServiceProvider
            .GetRequiredService<ExpiredOrderRecoveryService>();

        var recoveredCount = await recoveryService.RecoverExpiredOrdersAsync();

        Logger.LogInformation(
            "Expired order recovery completed. Recovered orders count: {RecoveredCount}",
            recoveredCount
        );
    }
}