namespace Defra.Identity.Services.Users;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Responses.Users;

public interface IUserService
{

    Task<User> Get(Expression<Func<UserAccount, bool>> predicate, CancellationToken cancellationToken = default);

    Task<User> Upsert(Requests.Users.User user, CancellationToken cancellationToken = default);
}
