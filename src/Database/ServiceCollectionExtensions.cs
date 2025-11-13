using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Livestock.Auth.Database;

public static class ServiceCollectionExtensions
{
    private const int MaxRetryCount = 5;
    private const int MaxRetryDelay = 10;

    private const int CommandTimeout = 60;

    /// <summary>
    /// Common PostgreSQL SQLSTATEs often considered transient:
    /// 40001: Serialization failure
    /// 40P01: Deadlock detected
    /// 55P03: Lock not available
    /// 53300: Too many connections
    /// 57014: Query canceled
    /// 57P01: Admin shutdown
    /// 57P02: Crash shutdown
    /// 57P03: Cannot connect now
    /// 58030: I/O error
    /// 08000/08003/08006/08001/08004/08007/08P01: Connection-related errors (connection exception class 08)
    /// </summary>
    private static readonly string[] s_errorCodes = ["40001", "40P01", "55P03", "57P03"];


    public static void AddAuthDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(Constants.ConnectionStringName);
        services
            .AddPooledDbContextFactory<AuthContext>((sp, options) =>
            {
                var env = sp.GetRequiredService<IHostEnvironment>();
                var isProd = env.IsProduction();
                options
                    .UseLoggerFactory(sp.GetRequiredService<ILoggerFactory>())
                    .UseNpgsql(
                        connectionString,
                        npgsqlOptions =>
                        {
                            npgsqlOptions.EnableRetryOnFailure(
                                maxRetryCount: MaxRetryCount,
                                maxRetryDelay: TimeSpan.FromSeconds(MaxRetryDelay),
                                errorCodesToAdd: s_errorCodes);
                            npgsqlOptions.CommandTimeout(CommandTimeout);
                        })
                    .EnableSensitiveDataLogging(isProd);
            });
        services.AddDbContext<AuthContext>();
    }

    public static void UseAuthDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AuthContext>>();
        using var context = factory.CreateDbContext();

        // Temporary Development migrations 
        var env = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();
      
        if (env.IsDevelopment()) context.Database.Migrate();
        
        if (context.Database.CanConnect())
            context.Database.OpenConnection();
    }
}