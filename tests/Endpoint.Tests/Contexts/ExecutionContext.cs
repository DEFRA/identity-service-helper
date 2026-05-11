// <copyright file="ExecutionContext.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Endpoint.Tests.Contexts;

using System.Net;

/// <summary>
/// The http client context.
/// </summary>
public class ExecutionContext
    : ITestContext
{
    /// <summary>
    /// Gets or sets the content of the request.
    /// </summary>
    /// <value>The content of the request.</value>
    public object? RequestContent { get; set; }

    /// <summary>
    /// Gets or sets the request content type.
    /// </summary>
    public string RequestContentType { get; set; } = null!;

    /// <summary>
    /// Gets or sets the content of the response.
    /// </summary>
    /// <value>The content of the response.</value>
    public object ResponseContent { get; set; } = null!;

    /// <summary>
    /// Gets or sets the Http Response received.
    /// </summary>
    /// <value>The HttpStatusCode instance.</value>
    public HttpStatusCode ResponseStatus { get; set; }

    /// <summary>
    /// Gets or sets the HttpRequestMessage instance.
    /// </summary>
    /// <value>The HttpRequestMessage instance.</value>
    public HttpRequestMessage HttpRequestMessage { get; set; } = null!;
}
