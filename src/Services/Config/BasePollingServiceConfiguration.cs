namespace Livestock.Auth.Services.Config;

public class BasePollingServiceConfiguration
{
    public required string CronSchedule { get; init; }
    
    public required string ServiceType { get; init; }
    
    public required string InterfaceType { get; init; }
    
    public required string ConfigurationType { get; init; }
    
    public required string Description { get; init; }
}