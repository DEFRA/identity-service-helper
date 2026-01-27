namespace Defra.Identity.Services.Users;

using Defra.Identity.Responses.Users;

public interface IUserService
{

    Task<User> Get(Guid id, CancellationToken cancellationToken = default);
}
