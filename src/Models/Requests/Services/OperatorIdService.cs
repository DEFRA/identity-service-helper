// <copyright file="OperatorIdService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Models.Requests.Services;

public class OperatorIdService : IOperatorIdService
{
    public Guid? OperatorId
    {
        get => field ?? throw new InvalidOperationException("Operator id has not been set");

        set
        {
            if (field.HasValue)
            {
                throw new InvalidOperationException("Operator id is already set");
            }

            field = value;
        }
    }
}
