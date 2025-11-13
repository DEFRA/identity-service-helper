using Livestock.Auth.Database.Entities.Base;

namespace Livestock.Auth.Database.Entities;

public class Enrolment : BaseUpdateEntity
{
    public required Guid UserAccountId { get;  init; }
    public UserAccount UserAccount { get; set; }
    public Guid ApplicationId { get; set; }
    public required string CphId { get; set; }
    public required string Role { get; set; }
    public string Scope { get; set; }
    
    public string Status { get; set; }
    public DateTime EnrolledAt { get; set; }
    public DateTime ExpiresAt { get; set; }
}