using Libro.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace Libro.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {

        public DbSet<User>? Users { get; set; }
        public DbSet<Role>? Roles { get; set; }
        public DbSet<UserRole>? UserRoles { get; set; }
        public DbSet<Book>? Books { get; set; }
        public DbSet<BookTransaction>? BookUsers { get; set; }
        public DbSet<Author>? Authors { get; set; }
        public DbSet<AuthorBook>? AuthorBooks { get; set; }
        public ApplicationDbContext() 
        {

        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Book>()
        .HasMany(a => a.Users)
        .WithMany(b => b.Books)
        .UsingEntity<BookTransaction>(
        join => join
        .HasOne<User>()
        .WithMany()
        .HasForeignKey(ca=>ca.UserId)
        .OnDelete(DeleteBehavior.Cascade),
       join => join
        .HasOne<Book>()
        .WithMany()
        .HasForeignKey(e => e.BookId)
        .OnDelete(DeleteBehavior.Cascade));

            modelBuilder.Entity<Book>()
          .HasMany(a => a.Authors)
          .WithMany(b => b.Books)
          .UsingEntity<AuthorBook>(
          join => join
          .HasOne<Author>()
          .WithMany()
          .HasForeignKey(ca => ca.AuthorId)
          .OnDelete(DeleteBehavior.Cascade),
         join => join
          .HasOne<Book>()
          .WithMany()
          .HasForeignKey(e => e.BookId)
          .OnDelete(DeleteBehavior.Cascade));

           modelBuilder.Entity<AuthorBook>().HasKey(e => new { e.BookId, e.AuthorId });

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

            modelBuilder.Entity<Role>().HasIndex(e => e.Name).IsUnique();
            modelBuilder.Entity<User>().HasIndex(e => e.Email).IsUnique();

            modelBuilder.Entity<UserRole>().HasKey(e => new { e.UserId, e.RoleId });

        }
    }
}
