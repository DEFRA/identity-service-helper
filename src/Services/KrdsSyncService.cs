using Livestock.Auth.Services.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;

namespace Livestock.Auth.Services;

public class KrdsSyncService(
    ILogger<KrdsSyncService> logger,
    IOptions<KrdsSyncServiceConfiguration> options)
    : IKrdsSyncService
{
    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation($"{nameof(KrdsSyncService)} starting");

        await Task.Delay(TimeSpan.FromSeconds(5), context.CancellationToken);

        logger.LogInformation($"{nameof(KrdsSyncService)} finished");
    }
}