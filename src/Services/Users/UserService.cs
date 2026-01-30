namespace Defra.Identity.Services.Users;

using System.Linq.Expressions;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories;
using Defra.Identity.Requests.Users.Queries;
using Defra.Identity.Responses.Users;
using Defra.Identity.Services;

public class UserService : IUserService
{

    private readonly IRepository<UserAccount> _repository;

    public UserService(IRepository<UserAccount> repository)
    {
        _repository = repository;
    }

    public async Task<User> Get(GetUser request, CancellationToken cancellationToken = default)
    {
       var userAccount = await _repository.Get(x => x.Id.Equals(request.Id), cancellationToken);

       if (userAccount == null)
       {
           return null;
       }

       var user = new User()
       {
           Id = userAccount.Id,
           Email = userAccount.EmailAddress,
           FirstName = userAccount.FirstName,
           LastName = userAccount.LastName,
       };
       return user;
    }

    public async Task<User> Upsert(Requests.Users.Commands.Update.UpdateUser user, CancellationToken cancellationToken = default)
    {
        var existingUser = await _repository.Get(x => x.EmailAddress.Equals(user.EmailAddress), cancellationToken);

        if (existingUser != null)
        {
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.EmailAddress = user.EmailAddress;
            var updated = await _repository.Update(existingUser, cancellationToken);

            return new User()
           {
               Id = updated.Id,
               Email = updated.EmailAddress,
               FirstName = updated.FirstName,
               LastName = updated.LastName,
           };
        }

        var userAccount = new UserAccount() { EmailAddress = user.EmailAddress, FirstName = user.FirstName, LastName = user.LastName };
        var result = await _repository.Create(userAccount, cancellationToken);
        return new User()
        {
            Id = result.Id,
            Email = result.EmailAddress,
            FirstName = result.FirstName,
            LastName = result.LastName,
        };
    }
}
