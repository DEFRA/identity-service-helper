namespace Defra.Identity.Requests.Users.Create;

using System.ComponentModel.DataAnnotations;
public class CreateUser : User
{

    public Guid CreatedBy { get; set; }
}
