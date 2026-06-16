// <copyright file="EntityExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Tests;

using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Defra.Identity.Postgres.Database.Entities;

[ExcludeFromCodeCoverage]
public static class EntityExtensions
{
    public static CountyParishHoldings WithCphAssignmentsFromTestData(
        this CountyParishHoldings cphEntity)
    {
        var cphAssignmentEntities = new List<UserAccountCountyParishHoldingAssignments>();

        GetCphAssignmentsForCph(typeof(TestData.CphAssignments), cphEntity.Id, cphAssignmentEntities);

        cphEntity.ApplicationUserAccountHoldingAssignments = cphAssignmentEntities;

        return cphEntity;
    }

    private static void GetCphAssignmentsForCph(
        Type targetType,
        Guid cphId,
        List<UserAccountCountyParishHoldingAssignments> cphAssignments)
    {
        const BindingFlags bindingFlags = BindingFlags.Static | BindingFlags.Public;

        cphAssignments.AddRange(from prop in targetType.GetProperties(bindingFlags)
            where prop.PropertyType == typeof(UserAccountCountyParishHoldingAssignments)
            select prop.GetValue(null) as UserAccountCountyParishHoldingAssignments
            into propertyValue
            where propertyValue?.CountyParishHoldingId == cphId
            select propertyValue!);

        foreach (var nestedType in targetType.GetNestedTypes(bindingFlags))
        {
            GetCphAssignmentsForCph(nestedType, cphId, cphAssignments);
        }
    }
}
