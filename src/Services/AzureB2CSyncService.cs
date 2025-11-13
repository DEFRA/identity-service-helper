using Livestock.Auth.Services.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;

namespace Livestock.Auth.Services;

public class AzureB2CSyncService(
    ILogger<AzureB2CSyncService> logger,
    IOptions<AzureB2CSyncServiceConfiguration> options)
    : IAzureB2CSyncService
{
    public async Task Execute(IJobExecutionContext context)
    {
        logger.LogInformation($"{nameof(AzureB2CSyncService)} starting");

        await Task.Delay(TimeSpan.FromSeconds(5), context.CancellationToken);

        logger.LogInformation($"{nameof(AzureB2CSyncService)} finished");
    }
}