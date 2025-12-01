// <copyright file="UsersRepository.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services;

using System.Linq.Expressions;
using Defra.Identity.Mongo.Database;
using Defra.Identity.Mongo.Database.Documents;
using MongoDB.Driver;

public class ApplicationsRepository(AuthMongoContext context, IMongoClient mongoClient)
    : IRepository<Applications>
{
   public async Task<List<Applications>> Get(Expression<Func<Applications, bool>> predicate, CancellationToken cancellationToken = default)
   {
       var query = await context.Applications.ToListAsync(cancellationToken: cancellationToken);
       return query.ToList<Applications>();
   }

    public Task<Applications> Create(Applications entity)
    {
        throw new NotImplementedException();
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
