using System.Linq.Expressions;
using Livestock.Auth.Database;
using Livestock.Auth.Database.Entities;

namespace Livestock.Auth.Services;

public class UsersRepository(AuthContext context): IRepository<UserAccount>
{
    public async Task<List<UserAccount>> GetAll()
    {
        var query = context.Users.AsQueryable();
        return await query.ToListAsync();
    }

    public async Task<UserAccount?> Get(Expression<Func<UserAccount, bool>> predicate)
    {
        var query = await context.Users.SingleOrDefaultAsync(predicate);
        return query ?? null;
    }

    public Task<UserAccount> Create(UserAccount entity)
    {
        throw new NotImplementedException();
    }

    public Task<UserAccount> Update(UserAccount entity)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Delete(Func<UserAccount, bool> predicate)
    {
        throw new NotImplementedException();
    }
}