// <copyright file="PredicateInterceptor.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Test.Utilities.Repository;

using System.Linq.Expressions;
using Defra.Identity.Repositories.Common;
using NSubstitute.Core;

public static class PredicateInterceptor
{
    public static TEntity? MockGetSingleEntityResult<TEntity>(CallInfo callInfo, TEntity entity)
    {
        return MockGetSingleEntityResult(callInfo, [entity]);
    }

    public static TEntity? MockGetSingleEntityResult<TEntity>(CallInfo callInfo, TEntity entity1, TEntity entity2)
    {
        return MockGetSingleEntityResult(callInfo, [entity1, entity2]);
    }

    public static TEntity? MockGetSingleEntityResult<TEntity>(CallInfo callInfo, TEntity[] entities)
    {
        var actualFilter = (Expression<Func<TEntity, bool>>)callInfo.Args()[0];
        var compiledFilter = actualFilter.Compile();

        var filteredEntity = entities.SingleOrDefault(compiledFilter);

        return filteredEntity;
    }

    public static PagedEntities<TEntity> MockGetAllPagedEntitiesResult<TEntity>(CallInfo callInfo, TEntity[] entities)
    {
        var actualFilter = (Expression<Func<TEntity, bool>>)callInfo.Args()[0];
        var actualOrderBy = (Expression<Func<TEntity, string>>)callInfo.Args()[3];

        var pageNumber = (int)callInfo.Args()[1];
        var pageSize = (int)callInfo.Args()[2];
        var orderByDescending = (bool)callInfo.Args()[4];

        var compiledFilter = actualFilter.Compile();
        var compiledOrderBy = actualOrderBy.Compile();

        var filteredEntities = entities.Where(compiledFilter);
        var orderedEntities = (orderByDescending ? filteredEntities.OrderByDescending(compiledOrderBy) : filteredEntities.OrderBy(compiledOrderBy)).ToList();

        var totalCount = orderedEntities.Count;
        var totalPages = (totalCount + pageSize - 1) / pageSize;

        var pagedEntities = orderedEntities
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedEntities<TEntity>(pagedEntities, totalCount, totalPages, pageNumber, pageSize);
    }

    public static PagedEntities<TAssociation> MockGetAllPagedAssociatedEntitiesResult<TPrimary, TAssociation>(
        CallInfo callInfo,
        TPrimary[] primaryEntities,
        TAssociation[] associatedEntities,
        Expression<Func<TPrimary, TAssociation, bool>> relationshipFilter)
    {
        var actualPrimaryFilter = (Expression<Func<TPrimary, bool>>)callInfo.Args()[0];
        var actualAssociationFilter = (Expression<Func<TAssociation, bool>>)callInfo.Args()[1];
        var actualOrderBy = (Expression<Func<TAssociation, string>>)callInfo.Args()[4];

        var pageNumber = (int)callInfo.Args()[2];
        var pageSize = (int)callInfo.Args()[3];
        var orderByDescending = (bool)callInfo.Args()[5];

        var compiledPrimaryFilter = actualPrimaryFilter.Compile();
        var compiledAssociationFilter = actualAssociationFilter.Compile();
        var compiledOrderBy = actualOrderBy.Compile();
        var compiledRelationshipFilter = relationshipFilter.Compile();

        var filteredPrimaryEntity = primaryEntities.FirstOrDefault(compiledPrimaryFilter);

        if (filteredPrimaryEntity == null)
        {
            return new PagedEntities<TAssociation>([], 0, 0, pageNumber, pageSize);
        }

        var queryableAssociations = associatedEntities.Where(entity => compiledRelationshipFilter(filteredPrimaryEntity, entity));
        var filteredAssociations = queryableAssociations.Where(compiledAssociationFilter);
        var orderedEntities = (orderByDescending ? filteredAssociations.OrderByDescending(compiledOrderBy) : filteredAssociations.OrderBy(compiledOrderBy)).ToList();

        var totalCount = orderedEntities.Count;
        var totalPages = (totalCount + pageSize - 1) / pageSize;

        var pagedEntities = orderedEntities
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedEntities<TAssociation>(pagedEntities, totalCount, totalPages, pageNumber, pageSize);
    }
}
