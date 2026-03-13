// <copyright file="ExpireCphByCphId.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Cphs.Commands;

using Defra.Identity.Requests.Common;

public class ExpireCphByCphId : IOperationById
{
    public Guid Id { get; set; }
}
