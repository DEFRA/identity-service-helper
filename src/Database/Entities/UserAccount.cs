using System.Data;
using Livestock.Auth.Database.Entities.Base;

namespace Livestock.Auth.Database.Entities;

public class UserAccount : BaseUpdateEntity
{
    public string Upn { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public bool AccountEnabled { get; set; }

    public ICollection<Federation> Federations { get; set; }
    public ICollection<Enrolment> Enrolments { get; set; }
 
}