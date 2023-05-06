﻿using Microsoft.EntityFrameworkCore;
using ProjectForVk.Core.Entities.DB;

namespace ProjectForVk.Infrastructure.Database;

internal class ApplicationContext : DbContext
{
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<UserGroupEntity> UserGroups { get; set; }
    public DbSet<UserStateEntity> UserStates { get; set; }
    
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options){}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserEntity>()
            .HasIndex(u => u.Login).IsUnique().HasDatabaseName("login");
    }
}