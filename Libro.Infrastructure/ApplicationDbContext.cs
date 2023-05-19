using Libro.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;


namespace Libro.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {

        public DbSet<User>? Users { get; set; }
        public DbSet<Role>? Roles { get; set; }
        public DbSet<UserRole>? UserRoles { get; set; }

        public ApplicationDbContext() 
        {

        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
  
     
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
          .HasMany(a => a.Roles)
          .WithMany(b => b.Users)
          .UsingEntity<UserRole>(
          join => join
          .HasOne<Role>()
          .WithMany()
          .HasForeignKey(ca => ca.RoleId)
          .OnDelete(DeleteBehavior.Cascade),
         join => join
          .HasOne<User>()
          .WithMany()
          .HasForeignKey(e => e.UserId)
          .OnDelete(DeleteBehavior.Cascade));

            modelBuilder.Entity<UserRole>().HasKey(e => new { e.UserId, e.RoleId });
       
        }
    }
}
