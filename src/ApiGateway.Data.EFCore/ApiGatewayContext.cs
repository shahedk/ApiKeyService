﻿using System;
using ApiGateway.Data.EFCore.Entity;
using Microsoft.EntityFrameworkCore;

namespace ApiGateway.Data.EFCore
{
    public class ApiGatewayContext : DbContext
    {
        public DbSet<Service> Services { get; set; }
        public DbSet<Key> Keys { get; set; }
        public DbSet<Api> Apis { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<ApiInRole> ApiInRoles { get; set; }
        public DbSet<KeyInRole> KeyInRoles { get; set; }
        
        public DbSet<AccessRule> AccessRules { get; set; }
        public DbSet<AccessRuleForRole> AccessRuleForRoles { get; set; }
        
        public ApiGatewayContext(DbContextOptions<ApiGatewayContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Key
            modelBuilder.Entity<Key>().HasIndex(x => x.PublicKey).IsUnique();
            modelBuilder.Entity<Key>().HasIndex(x => x.OwnerKeyId).IsUnique(false);
             
            // Service
            modelBuilder.Entity<Service>().HasIndex(x => new {x.OwnerKeyId, x.Name});
            modelBuilder.Entity<Service>().HasOne(x => x.OwnerKey).WithMany(x => x.Services)
                .HasForeignKey(x => x.OwnerKeyId).HasConstraintName("ForeignKey_OwnerKey_Service");
            
            // Api
            modelBuilder.Entity<Api>().HasIndex(x => new {x.ServiceId, x.Url, x.HttpMethod}).IsUnique();
            modelBuilder.Entity<Api>().HasIndex(x => x.ServiceId).IsUnique(false);
            modelBuilder.Entity<Api>().HasOne(x => x.OwnerKey).WithMany(x => x.Apis)
                .HasForeignKey(x => x.OwnerKeyId).HasConstraintName("ForeignKey_OwnerKey_Api");
            
            // Role
            modelBuilder.Entity<Role>().HasIndex(x => new {x.ServiceId, x.Name}).IsUnique();
            modelBuilder.Entity<Role>().HasOne(x => x.Service).WithMany(x => x.Roles).HasForeignKey(x => x.ServiceId)
                .HasConstraintName("ForeignKey_Role_Service");

            // ApiInRole (Many to Many)
            modelBuilder.Entity<ApiInRole>().HasIndex(x => new{x.ApiId, x.RoleId});
            modelBuilder.Entity<ApiInRole>().HasOne(x => x.OwnerKey).WithMany(x => x.ApiInRoles)
                .HasForeignKey(x => x.OwnerKeyId).HasConstraintName("ForeignKey_OwnerKey_ApiInRole");

            // KeyInRole (Many to Many)
            modelBuilder.Entity<KeyInRole>().HasIndex(x => x.KeyId).IsUnique(false);
            modelBuilder.Entity<KeyInRole>().HasIndex(x => new {x.KeyId, x.RoleId}).IsUnique();
            modelBuilder.Entity<KeyInRole>().HasOne(x => x.OwnerKey).WithMany(x => x.KeyInRoles)
                .HasForeignKey(x => x.OwnerKeyId).HasConstraintName("ForeignKey_OwnerKey_KeyInRole");
            
            // AccessRule
            modelBuilder.Entity<AccessRule>().HasIndex(x => x.OwnerKeyId).IsUnique(false);
            modelBuilder.Entity<AccessRule>().HasIndex(x => new{x.ServiceId, x.Name}).IsUnique();
            modelBuilder.Entity<AccessRule>().HasOne(x => x.OwnerKey).WithMany(x => x.AccessRules)
                .HasForeignKey(x => x.OwnerKeyId).HasConstraintName("ForeignKey_OwnerKey_AccessRule");

            //AccessRuleForRoles
            modelBuilder.Entity<AccessRuleForRole>().HasIndex(x => x.OwnerKeyId).IsUnique(false);
            modelBuilder.Entity<AccessRuleForRole>().HasIndex(x => new { x.RoleId, x.AcccessRuleId }).IsUnique();
            modelBuilder.Entity<AccessRuleForRole>().HasOne(x => x.OwnerKey).WithMany(x => x.AccessRuleForRoles)
                .HasForeignKey(x => x.OwnerKeyId).HasConstraintName("ForeignKey_OwnerKey_AccessRuleForRole");
        }
    }
}