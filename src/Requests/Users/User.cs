namespace Defra.Identity.Requests.Users;

public abstract class User
{
    public string DisplayName { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;
}
