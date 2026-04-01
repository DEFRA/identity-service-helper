// <copyright file="CphService.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Cphs;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Common;
using Defra.Identity.Repositories.Cphs;
using Defra.Identity.Repositories.Exceptions;
using Defra.Identity.Requests.Cphs.Commands;
using Defra.Identity.Requests.Cphs.Common;
using Defra.Identity.Requests.Cphs.Queries;
using Defra.Identity.Responses.Common;
using Defra.Identity.Responses.Cphs;
using Defra.Identity.Services.Exceptions;
using FluentValidation;
using Microsoft.Extensions.Logging;

public class CphService : ICphService
{
    private readonly ICphRepository cphRepository;
    private readonly ICphUsersRepository cphUsersRepository;
    private readonly IValidator<IOperationByCphNumber> cphNumberValidator;
    private readonly ILogger<CphService> logger;

    public CphService(ICphRepository cphRepository, ICphUsersRepository cphUsersRepository, IValidator<IOperationByCphNumber> cphNumberValidator, ILogger<CphService> logger)
    {
        this.cphRepository = cphRepository;
        this.cphUsersRepository = cphUsersRepository;
        this.cphNumberValidator = cphNumberValidator;
        this.logger = logger;
    }

    public async Task<Guid> GetIdFromCphNumber(IOperationByCphNumber request, CancellationToken cancellationToken = default)
    {
        var cphNumberValidationResult = await cphNumberValidator.ValidateAsync(request, cancellationToken);

        if (!cphNumberValidationResult.IsValid)
        {
            throw new ValidationException(cphNumberValidationResult.Errors);
        }

        var formattedCphNumber = $"{request.County:D2}/{request.Parish:D3}/{request.Holding:D4}";

        logger.LogInformation("Getting county parish holding id by cph number {FormattedCphNumber}", formattedCphNumber);

        Expression<Func<CountyParishHoldings, bool>> filter = cph => cph.Identifier == formattedCphNumber;

        var cphEntity = await cphRepository.GetSingle(filter, cancellationToken);

        if (cphEntity is not { DeletedAt: null })
        {
            logger.LogWarning("County parish holding with cph number {FormattedCphNumber} not found", formattedCphNumber);

            throw new NotFoundException("County parish holding not found.");
        }

        return cphEntity.Id;
    }

    public async Task<PagedResults<Cph>> GetAllPaged(GetCphs request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting all county parish holdings by page");

        var includeExpired = IsExpiredInferred(request);

        Expression<Func<CountyParishHoldings, bool>> filter = cph => (includeExpired || cph.ExpiredAt == null) && cph.DeletedAt == null;
        Expression<Func<CountyParishHoldings, string>> orderBy = cph => cph.Identifier;

        var pagedCphEntities = await cphRepository.GetPaged(filter, request.PageNumber, request.PageSize, orderBy, request.OrderByDescending ?? false, cancellationToken);
        var pagedCphResults = pagedCphEntities.ToPagedResults(MapCphEntityToCph);

        return pagedCphResults;
    }

    public async Task<Cph> Get(GetCphByCphId request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting county parish holding by id {Id}", request.Id);

        Expression<Func<CountyParishHoldings, bool>> filter = cph => cph.Id == request.Id;

        var cphEntity = await cphRepository.GetSingle(filter, cancellationToken);

        if (cphEntity is not { DeletedAt: null })
        {
            logger.LogWarning("County parish holding with id {Id} not found", request.Id);

            throw new NotFoundException("County parish holding not found.");
        }

        var cphResult = MapCphEntityToCph(cphEntity);

        return cphResult;
    }

    public async Task Expire(ExpireCphByCphId request, Guid operatorId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Expiring county parish holding with id {Id} by operator {OperatorId}", request.Id, operatorId);

        Expression<Func<CountyParishHoldings, bool>> filter = cph => cph.Id == request.Id;

        var cphEntity = await cphRepository.GetSingle(filter, cancellationToken);

        if (cphEntity is not { DeletedAt: null })
        {
            logger.LogWarning("County parish holding with id {Id} not found", request.Id);

            throw new NotFoundException("County parish holding not found.");
        }

        if (cphEntity.ExpiredAt != null)
        {
            logger.LogWarning("County parish holding with id {Id} is already expired", request.Id);

            throw new ConflictException("County parish holding already expired.");
        }

        cphEntity.ExpiredAt = DateTime.UtcNow;

        await cphRepository.Update(cphEntity, cancellationToken);
    }

    public async Task Delete(DeleteCphByCphId request, Guid operatorId, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Deleting county parish holding with id {Id} by operator {OperatorId}", request.Id, operatorId);

        Expression<Func<CountyParishHoldings, bool>> filter = cph => cph.Id == request.Id;

        var cphEntity = await cphRepository.GetSingle(filter, cancellationToken);

        if (cphEntity is not { DeletedAt: null })
        {
            logger.LogWarning("County parish holding with id {Id} not found", request.Id);

            throw new NotFoundException("County parish holding not found.");
        }

        cphEntity.DeletedById = operatorId;
        cphEntity.DeletedAt = DateTime.UtcNow;

        await cphRepository.Update(cphEntity, cancellationToken);
    }

    public async Task<PagedResults<CphUser>> GetAllCphUsersPaged(GetCphUsersByCphId request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting all county parish holding users for id {Id} by page", request.Id);

        Expression<Func<CountyParishHoldings, bool>> primaryFilter = cph => cph.Id == request.Id;
        Expression<Func<ApplicationUserAccountHoldingAssignments, bool>> associationFilter = cphUser => cphUser.DeletedAt == null;
        Expression<Func<ApplicationUserAccountHoldingAssignments, string>> orderBy = cphUser => cphUser.UserAccount.DisplayName;

        var cphEntity = await cphRepository.GetSingle(primaryFilter, cancellationToken);

        if (cphEntity is not { DeletedAt: null })
        {
            logger.LogWarning("County parish holding with id {Id} not found", request.Id);

            throw new NotFoundException("County parish holding not found.");
        }

        var pageCphUserEntities = await cphUsersRepository.GetPaged(
            primaryFilter,
            associationFilter,
            request.PageNumber,
            request.PageSize,
            orderBy,
            request.OrderByDescending ?? false,
            cancellationToken);

        var pagedCphUserResults = pageCphUserEntities.ToPagedResults(MapCphUserEntityToCphUser);

        return pagedCphUserResults;
    }

    public async Task<Cph> Upsert(
        CountyParishHoldings countyParishHolding,
        CancellationToken cancellationToken = default)
    {
        var cph = await cphRepository.GetSingle(x => x.Identifier.Equals(countyParishHolding.Identifier), cancellationToken);
        if (cph != null)
        {
            return MapCphEntityToCph(cph);
        }

        var newCph = new CountyParishHoldings

        {
            Identifier = countyParishHolding.Identifier,
        };
        return MapCphEntityToCph(await cphRepository.Create(newCph, cancellationToken));
    }

    private static Cph MapCphEntityToCph(CountyParishHoldings cphEntity)
    {
        return new Cph
        {
            Id = cphEntity.Id, CphNumber = cphEntity.Identifier, Expired = cphEntity.ExpiredAt != null, ExpiredAt = cphEntity.ExpiredAt,
        };
    }

    private static CphUser MapCphUserEntityToCphUser(ApplicationUserAccountHoldingAssignments cphUserEntity)
    {
        return new CphUser
        {
            Id = cphUserEntity.Id,
            UserId = cphUserEntity.UserAccountId,
            ApplicationId = cphUserEntity.ApplicationId,
            RoleId = cphUserEntity.RoleId,
            Email = cphUserEntity.UserAccount.EmailAddress,
            DisplayName = cphUserEntity.UserAccount.DisplayName,
        };
    }

    private static bool IsExpiredInferred(GetCphs request)
    {
        return request.Expired != null && (request.Expired == string.Empty || request.Expired.Equals("true", StringComparison.InvariantCultureIgnoreCase));
    }
}
