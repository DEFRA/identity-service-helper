// <copyright file="IOperatorIdService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Services;

public interface IOperatorIdService
{
    Guid? OperatorId { get; set; }
}
