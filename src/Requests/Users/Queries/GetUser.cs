namespace Defra.Identity.Requests.Users.Queries;

public class GetUser
{
    public Guid Id { get; set; }

    public string? Status { get; set; } = "Active";
}
