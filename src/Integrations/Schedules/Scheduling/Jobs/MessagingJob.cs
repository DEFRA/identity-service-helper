// <copyright file="MessagingJob.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Scheduling.Jobs;

using Defra.Identity.Messaging;
using Defra.Identity.Scheduling.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Quartz;

[DisallowConcurrentExecution]
public class MessagingJob(
    ILogger<MessagingJob> logger,
    IOptions<MessagingSchedulingOptions> options,
    IMessageQueueProcessor messageQueueProcessor)
    : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            logger.LogInformation(
                "{Job} starting {Date}",
                context.JobDetail.Key.Name,
                DateTime.UtcNow);

            var result = await messageQueueProcessor
                .ProcessMessageQueueAsync(context.CancellationToken)
                .ConfigureAwait(false);

            logger.LogInformation(
                "Processed Email {SuccessEmailCount}-S, {ErrorEmailCount}-F, Text {SuccessSmsCount}-S, {ErrorSmsCount}-F",
                result.Success.EmailCountProcessed,
                result.Error.EmailCountProcessed,
                result.Success.SmsCountProcessed,
                result.Error.SmsCountProcessed);
        }
        catch (OperationCanceledException ex)
        {
            logger.LogWarning(ex, "{Job} cancelled.", context.JobDetail.Key.Name);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Job} failed", context.JobDetail.Key.Name);
            throw;
        }
    }
}
