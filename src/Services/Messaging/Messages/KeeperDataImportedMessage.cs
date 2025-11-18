// <copyright file="KeeperDataImportedMessage.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Livestock.Auth.Services.Messaging.Messages;

public class KeeperDataImportedMessage
{
    public required string Type { get; set; }

    public required string MessageId { get; set; }

    public required string TopicArn { get; set; }

    public required string Message { get; set; }

    public required string Timestamp { get; set; }

    public required string UnsubscribeURL { get; set; }

    public required string SignatureVersion { get; set; }

    public required string Signature { get; set; }

    public required string SigningCertURL { get; set; }
}