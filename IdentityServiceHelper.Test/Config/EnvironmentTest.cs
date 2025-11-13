using Microsoft.AspNetCore.Builder;

namespace IdentityServiceHelper.Test.Config;

public class EnvironmentTest
{

   [Fact]
   public void IsNotDevModeByDefault()
   { 
       var builder = WebApplication.CreateEmptyBuilder(new WebApplicationOptions());
       var isDev = IdentityServiceHelper.Config.Environment.IsDevMode(builder);
       Assert.False(isDev);
   }
}
