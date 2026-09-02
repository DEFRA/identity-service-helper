// <copyright file="UserAccountMapper.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Common.Mappers;

using Defra.Identity.Models.Responses.UserAccounts;
using KrdsCphAssociation = Defra.Identity.KeeperReferenceData.Models.UserAccounts.CphAssociation;
using KrdsUserAccount = Defra.Identity.KeeperReferenceData.Models.UserAccounts.UserAccount;

public static class UserAccountMapper
{
    public static UserAccount MapKrdsUserAccountToUserAccount(KrdsUserAccount account)
    {
        ArgumentNullException.ThrowIfNull(account);

        return new UserAccount
        {
            Id = account.Id,
            Subject = account.Subject,
            Email = account.Email,
            FirstName = account.FirstName,
            LastName = account.LastName,
            DisplayName = account.DisplayName,
            AssociationsRefreshedAt = account.AssociationsRefreshedDate,
            CphAssociations = account.CphAssociations
                .Select(MapKrdsCphAssociationToCphAssociation)
                .ToList(),
        };
    }

    private static CphAssociation MapKrdsCphAssociationToCphAssociation(KrdsCphAssociation association)
    {
        return new CphAssociation
        {
            Id = association.Id,
            CphNumber = association.CphNumber,
            Role = association.Role,
            PartyId = association.PartyId,
            HoldingId = association.HoldingId,
            HoldingName = association.HoldingName,
        };
    }
}
