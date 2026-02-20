// <copyright file="CphServiceTestDataHelper.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Tests.Cphs.TestData;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Common;
using NSubstitute.Core;

public static class CphServiceTestDataHelper
{
    public static PagedEntities<CountyParishHoldings> MockGetAllPagedEntitiesResultFromCallInfo(CallInfo x)
    {
        var actualFilter = (Expression<Func<CountyParishHoldings, bool>>)x.Args()[0];
        var actualOrderBy = (Expression<Func<CountyParishHoldings, string>>)x.Args()[3];

        var pageNumber = (int)x.Args()[1];
        var pageSize = (int)x.Args()[2];
        var orderByDescending = (bool)x.Args()[4];

        var compiledFilter = actualFilter.Compile();
        var compiledOrderBy = actualOrderBy.Compile();

        var filteredEntities = GetCphEntities().Where(compiledFilter);
        var orderedEntities = (orderByDescending ? filteredEntities.OrderByDescending(compiledOrderBy) : filteredEntities.OrderBy(compiledOrderBy)).ToList();

        var totalCount = orderedEntities.Count;
        var totalPages = (totalCount + pageSize - 1) / pageSize;

        var pagedEntities = orderedEntities
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return new PagedEntities<CountyParishHoldings>(pagedEntities, totalCount, totalPages, pageNumber, pageSize);
    }

    private static CountyParishHoldings[] GetCphEntities() =>
    [
        new()
        {
            Id = new Guid("7140056b-b2ee-40d6-9be1-882bdff30cc2"), Identifier = $"44/100/0007", ExpiredAt = null, DeletedAt = null,
        },
        new()
        {
            Id = new Guid("1cd09a5b-6b00-4f30-b03e-8de45130cad6"), Identifier = $"44/100/0001", ExpiredAt = null, DeletedAt = null,
        },
        new()
        {
            Id = new Guid("a4343f59-011c-46dc-a9fe-553923338e0a"), Identifier = $"44/100/0003", ExpiredAt = null, DeletedAt = DateTime.Parse("2026-02-13").ToUniversalTime(),
        },
        new()
        {
            Id = new Guid("e7009d6d-0a29-4e3f-ac0b-7bf0c7497f46"), Identifier = $"44/100/0002", ExpiredAt = DateTime.Parse("2026-02-12").ToUniversalTime(), DeletedAt = null,
        },
        new()
        {
            Id = new Guid("5bc8f1a5-2d44-40b5-93e4-52b613bf099f"), Identifier = $"44/100/0006", ExpiredAt = null, DeletedAt = DateTime.Parse("2026-02-11").ToUniversalTime(),
        },
        new()
        {
            Id = new Guid("77b9c956-2780-4b48-9abc-71bf505466f9"), Identifier = $"44/100/0004", ExpiredAt = null, DeletedAt = null,
        },
        new()
        {
            Id = new Guid("802428bd-0411-451b-b75c-2fb6c037f271"), Identifier = $"44/100/0005", ExpiredAt = DateTime.Parse("2026-02-10").ToUniversalTime(), DeletedAt = null,
        },
    ];
}
