// <copyright file="IOperationById.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Common;

public interface IOperationById<T> : ILoggableById
    where T : IComparable
{
    T Id { get; set; }
}
