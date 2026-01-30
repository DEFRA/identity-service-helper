namespace Defra.Identity.Requests.Users.Commands.Create;

public class CreateUser : User
{
    public Guid OperatorId { get; set; }

    public string EmailAddress { get; set; }
}
