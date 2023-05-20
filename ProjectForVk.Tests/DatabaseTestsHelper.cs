using Microsoft.EntityFrameworkCore;
using ProjectForVk.Infrastructure.Database;

namespace ProjectForVk.Tests;

[Collection("Db collection")]
public abstract class DatabaseTestsHelper
{
    internal static ApplicationContext CreateContext()
    {
        var builder = new DbContextOptionsBuilder<ApplicationContext>()
            .UseNpgsql(PostgresContainerFixture.ConnectionString);
        var dbContext = new ApplicationContext(builder.Options);

        return dbContext;
    }

    internal static async Task ClearDatabase(ApplicationContext context)
    {
        await context.Users.ExecuteDeleteAsync();
        await context.UserGroups.ExecuteDeleteAsync();
        await context.UserStates.ExecuteDeleteAsync();
    }
}