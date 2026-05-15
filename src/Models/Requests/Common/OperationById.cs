// <copyright file="OperationById.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Common;

using System.ComponentModel;

public abstract class OperationById<T> : IOperationById<T>
    where T : IComparable
{
    [Description(OpenApiMetadata.Generic.Id)]
    public T Id { get; set; }

    public string GetLoggableId()
    {
        return Id?.ToString() ?? throw new InvalidOperationException("Id has not been set");
    }
}
