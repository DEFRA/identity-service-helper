// <copyright file="ErrorResponse.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Messaging.Models.Response;

using System.Net;
using Newtonsoft.Json;

public class ErrorResponse
{
    [JsonProperty("status_code")]
    public HttpStatusCode StatusCode { get; set; }

    public ErrorItem[] Errors { get; set; }
}
