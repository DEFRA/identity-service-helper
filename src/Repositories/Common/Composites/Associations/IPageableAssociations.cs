// <copyright file="IPageableAssociation.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Common.Composites.Associations;

using System.Linq.Expressions;

public interface IPageableAssociations<TPrimary, TAssociation>
    where TPrimary : class
{
    Task<PagedEntities<TAssociation>> GetPaged<TOrderBy>(
        Expression<Func<TPrimary, bool>> primaryPredicate,
        Expression<Func<TAssociation, bool>> associationsPredicate,
        int pageNumber,
        int pageSize,
        Expression<Func<TAssociation, TOrderBy>> orderBy,
        bool orderByDescending,
        CancellationToken cancellationToken = default);
}
