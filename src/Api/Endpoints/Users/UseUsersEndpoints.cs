using Livestock.Auth.Database.Entities;
using Livestock.Auth.Services;

namespace Livestock.Auth.Api.Endpoints.Users;

public static class UsersEndpoints
{
    public  static void UseUsersEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet(RouteNames.Users, GetAll);
        app.MapGet(RouteNames.Users + "/{id:guid}", Get);
    }
    private static async Task<IResult> GetAll(
        IRepository<UserAccount> service)
    {
        var matches = await service.GetAll();
        return Results.Ok(matches);
    }
    
    private static async Task<IResult> Get(Guid id,
        IRepository<UserAccount> service)
    {
        var matches = await service.Get(x => x.Id.Equals(id));
        return Results.Ok(matches);
    }
}