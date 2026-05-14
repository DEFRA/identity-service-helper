// <copyright file="KrdsProvider.logger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Providers;

using System.Text.Json;
using Microsoft.Extensions.Logging;

public partial class KrdsProvider
{
    [LoggerMessage(LogLevel.Information, "Getting parties")]
    partial void LogGettingParties();

    [LoggerMessage(LogLevel.Information, "Request: {Request}")]
    partial void LogRequestRequest(string request);

    [LoggerMessage(LogLevel.Error, "Error deserializing parties. Body: {ResponseBody}")]
    partial void LogErrorDeserializingPartiesBodyResponsebody(string responseBody, JsonException jsonException);
}
