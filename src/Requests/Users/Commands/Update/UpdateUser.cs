namespace Defra.Identity.Requests.Users.Commands.Update;

public class UpdateUser : User
{
    public Guid OperatorId { get; set; }

    public string EmailAddress { get; set; }
}
