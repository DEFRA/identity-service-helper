using Livestock.Auth.Database.Entities.Base;

namespace Livestock.Auth.Database.Entities;

public class Federation : BaseUpdateEntity
{
    public required Guid UserAccountId { get; set; }
    public required UserAccount UserAccount { get; set; }
    public required string TenantName { get; set; } 
    public required Guid ObjectId { get; set; }
    public required string TrustLevel { get; set; }
    public required string SyncStatus { get; set; }
    public required DateTime LastSyncedAt { get; set; }

    
}