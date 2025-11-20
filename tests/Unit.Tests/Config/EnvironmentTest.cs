using Environment = Defra.Identity.Api.Config.Environment;

namespace Defra.Identity.Unit.Tests.Config;

using Microsoft.AspNetCore.Builder;

public class EnvironmentTest
{

   [Fact]
   public void IsNotDevModeByDefault()
   { 
       var builder = WebApplication.CreateEmptyBuilder(new WebApplicationOptions());
       var isDev = Environment.IsDevMode(builder);
       Assert.False(isDev);
   }
}
