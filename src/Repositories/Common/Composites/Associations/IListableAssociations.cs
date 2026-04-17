// <copyright file="IListableAssociations.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Common.Composites.Associations;

using System.Linq.Expressions;

public interface IListableAssociations<TPrimary, TAssociation>
    where TPrimary : class
    where TAssociation : class
{
    Task<List<TAssociation>> GetList(
        Expression<Func<TPrimary, bool>> primaryPredicate,
        Expression<Func<TAssociation, bool>> associationsPredicate,
        CancellationToken cancellationToken = default);
}
