﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectForVk.Application.Services;
using ProjectForVk.Infrastructure.Database;
using ProjectForVk.Infrastructure.Services;

namespace ProjectForVk.Infrastructure.DI;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // services.AddDbContext<ApplicationContext>(options => 
        //     options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddDbContext<ApplicationContext>(options =>
            options.UseInMemoryDatabase("test"));

        services.AddScoped<IUserService, UserService>();


        return services;
    }
}