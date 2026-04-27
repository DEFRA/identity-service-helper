// <copyright file="MessageResponse.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Responses.Messaging;

using System.Net;
using Newtonsoft.Json;

public class MessageResponse
{
    public string NotifyId { get; set; }

    [JsonProperty("status_code")]
    public HttpStatusCode Status { get; set; }

    public string Recipient { get; set; }

    public MessageTypes MessageType { get; set; }

    public string TemplateId { get; set; }

    public ErrorItem[] Errors { get; set; }

    public bool IsSuccess => Status == HttpStatusCode.OK;
}
