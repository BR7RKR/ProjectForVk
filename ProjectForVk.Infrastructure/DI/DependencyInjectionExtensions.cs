using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectForVk.Infrastructure.Database;

namespace ProjectForVk.Infrastructure.DI;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // services.AddDbContext<ApplicationContext>(options => 
        //     options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddDbContext<ApplicationContext>(options =>
            options.UseInMemoryDatabase("test"));

        return services;
    }
}