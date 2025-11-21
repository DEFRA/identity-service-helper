// <copyright file="SnsEnvelope.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.MessageProcessor.Messages;

using Newtonsoft.Json;

public class SnsEnvelope
{
    public required string Type { get; set; }

    public required string MessageId { get; set; }

    public required string TopicArn { get; set; }

    public required string Message { get; set; }

    public required string Timestamp { get; set; }

    public required string SignatureVersion { get; set; }

    public required string Signature { get; set; }

    [JsonProperty("SigningCertURL")]
    public required string SigningCertUrl { get; set; }

    [JsonProperty("UnsubscribeURL")]
    public required string UnsubscribeUrl { get; set; }
}
