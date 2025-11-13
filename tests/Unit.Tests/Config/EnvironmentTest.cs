using Microsoft.AspNetCore.Builder;
using Environment = Livestock.Auth.Api.Config.Environment;

namespace Livestock.Auth.Config;

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
