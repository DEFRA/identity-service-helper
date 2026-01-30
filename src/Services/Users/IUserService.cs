namespace Defra.Identity.Services.Users;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Requests.Users.Queries;
using Defra.Identity.Responses.Users;

public interface IUserService
{

    Task<User> Get(GetUser request, CancellationToken cancellationToken = default);

    Task<User> Upsert(Requests.Users.Commands.Update.UpdateUser user, CancellationToken cancellationToken = default);
}
