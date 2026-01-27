namespace Defra.Identity.Services.Users;

using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Responses.Users;
using Defra.Identity.Services;

public class UserService : IUserService
{

    private readonly IRepository<UserAccount> _repository;

    public UserService(IRepository<UserAccount> repository)
    {
        _repository = repository;
    }

    public async Task<User> Get(Guid id, CancellationToken cancellationToken = default)
    {
       var userAccount = await _repository.Get(x => x.Id.Equals(id), cancellationToken);

       var user = new User()
       {
           Id = userAccount.Id,
           EmailAddress = userAccount.EmailAddress,
           FirstName = userAccount.FirstName,
           LastName = userAccount.LastName,
       };
       return user;
    }
}
