using Livestock.Auth.Database.Entities.Base;
using Microsoft.AspNetCore.Identity;

namespace Livestock.Auth.Database.Entities;

public class KrdsSyncLog : BaseProcessingEntity
{
   
    public Guid CorrelationId { get; set; }
    public string Upn { get; set; }
    public string PayloadSha256 { get; set; }
    public string SourceEndpoint { get; set; }
    public int HttpStatus { get; set; }
    public bool ProcessedOk { get; set; }
    public string Message { get; set; }
  
}