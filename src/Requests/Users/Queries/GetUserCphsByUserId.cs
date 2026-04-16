// <copyright file="GetUserCphsByUserId.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests.Users.Queries;

using Defra.Identity.Requests.Common;

public class GetUserCphsByUserId : IOperationById
{
    public Guid Id { get; set; }
}
