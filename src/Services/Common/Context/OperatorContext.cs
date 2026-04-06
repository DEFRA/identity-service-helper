// <copyright file="OperatorContext.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Context;

using Defra.Identity.Requests.Services;

public class OperatorContext(IOperatorIdService operatorIdService) : IOperatorContext
{
    public Guid OperatorId => operatorIdService.OperatorId!.Value;
}
