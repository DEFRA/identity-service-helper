// <copyright file="GetCphByCphId.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Cphs.Queries;

using Defra.Identity.Requests.Common;

public class GetCphByCphId : IOperationById
{
    public Guid Id { get; set; }
}
