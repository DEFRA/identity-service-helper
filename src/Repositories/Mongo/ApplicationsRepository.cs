// <copyright file="ApplicationsRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories.Mongo;

using System.Linq.Expressions;
using Defra.Identity.Mongo.Database;
using Defra.Identity.Mongo.Database.Documents;
using MongoDB.Driver.Linq;

public class ApplicationsRepository(AuthMongoContext context)
    : IRepository<Applications>
{
   public async Task<List<Applications>> Get(Expression<Func<Applications, bool>> predicate, CancellationToken cancellationToken = default)
   {
       var query = await context.Applications.ToListAsync(cancellationToken: cancellationToken);
       return query.ToList<Applications>();
   }

   public async Task<Applications> Create(Applications entity, CancellationToken cancellationToken = default)
    {
        await context.Applications.AddAsync(entity, CancellationToken.None);
        await context.SaveChangesAsync(cancellationToken);
        return entity;
    }

   public Task<Applications> Update(Applications entity)
    {
        throw new NotImplementedException();
    }

   public Task<bool> Delete(Func<Applications, bool> predicate)
    {
        throw new NotImplementedException();
    }
}
