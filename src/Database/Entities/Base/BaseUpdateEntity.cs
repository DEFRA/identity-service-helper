namespace Livestock.Auth.Database.Entities.Base;

public abstract class BaseUpdateEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}