namespace Livestock.Auth.Database.Entities.Base;

public abstract class BaseProcessingEntity
{
    public Guid Id { get; set; }
    public DateTime ReceivedAt { get; set; }
    public DateTime ProcessedAt { get; set; }
}