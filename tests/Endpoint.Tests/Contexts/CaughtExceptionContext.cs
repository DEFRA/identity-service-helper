// <copyright file="CaughtExceptionContext.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Endpoint.Tests.Contexts;

using System;

/// <summary>
/// Caught Exception Context.
/// </summary>
public class CaughtExceptionContext
    : ITestContext
{
    /// <summary>
    /// Gets or sets the caught exception.
    /// </summary>
    /// <value>The caught exception.</value>
    public Exception? CaughtException { get; set; }

    /// <summary>
    /// Gets or sets the type of exception under test.
    /// </summary>
    /// <value>
    /// The type of exception under test.
    /// </value>
    public Type? ExceptionType { get; set; }

    /// <summary>
    /// Gets or sets the inner exception to store inside the exception.
    /// </summary>
    /// <value>
    /// The inner exception to store inside the exception.
    /// </value>
    public Exception? InnerException { get; set; }

    /// <summary>
    /// Gets or sets the message to store inside the exception.
    /// </summary>
    /// <value>
    /// The message to store inside the exception.
    /// </value>
    public string? Message { get; set; }
}
