// <copyright file="StopwatchLogger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Infrastructure.Monitoring;

using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

public class StopwatchLogger : IDisposable
{
    private readonly Stopwatch sw;
    private readonly ILogger logger;
    private readonly string memberName;
    private readonly string className;

    public StopwatchLogger(
        ILogger logger,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string filePath = "")
    {
        this.logger = logger;
        sw = Stopwatch.StartNew();
        className = Path.GetFileNameWithoutExtension(filePath);
        this.memberName = memberName;

        logger.LogInformation("Started {ClassName}.{MemberName}", className, this.memberName);
    }

    public void Dispose()
    {
        sw.Stop();
        logger.LogInformation("Finished {ClassName}.{MemberName} ElapsedTime:{ElapsedTime}", className, memberName, sw.Elapsed);
    }
}
