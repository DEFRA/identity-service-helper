namespace Defra.Identity.Requests.Users.Commands.Update;

public class UpdateUser : User
{
    public Guid Id { get; set; }

    public string Email { get; set; } = string.Empty;
}
