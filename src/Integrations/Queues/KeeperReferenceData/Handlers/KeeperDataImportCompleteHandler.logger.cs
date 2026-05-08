// <copyright file="KeeperDataImportCompleteHandler.logger.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.KeeperReferenceData.Handlers;

using Microsoft.Extensions.Logging;

public partial class KeeperDataImportCompleteHandler
{
    [LoggerMessage(LogLevel.Information, "Processing KeeperDataImportComplete message.")]
    partial void LogProcessingKeeperDataImportCompleteMessage();
}
