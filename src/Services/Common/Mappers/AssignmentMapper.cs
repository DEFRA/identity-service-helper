// <copyright file="AssignmentMapper.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Mappers;

using Defra.Identity.Models.Responses.Assignments;
using Defra.Identity.Postgres.Database.Entities;

public static class AssignmentMapper
{
    public static CphAssignment MapCphAssignmentEntityToCphAssignment(ApplicationUserAccountHoldingAssignments cphAssignmentEntity)
    {
        return new CphAssignment
        {
            Id = cphAssignmentEntity.Id,
            CountyParishHoldingId = cphAssignmentEntity.CountyParishHoldingId,
            CountyParishHoldingNumber = cphAssignmentEntity.CountyParishHolding.Identifier,
            UserId = cphAssignmentEntity.UserAccountId,
            ApplicationId = cphAssignmentEntity.ApplicationId,
            RoleId = cphAssignmentEntity.RoleId,
            RoleName = cphAssignmentEntity.Role.Name,
            Email = cphAssignmentEntity.UserAccount.EmailAddress,
            DisplayName = cphAssignmentEntity.UserAccount.DisplayName,
        };
    }
}
