using Microsoft.EntityFrameworkCore;


namespace Libro.Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        //public DbSet<Enemy>? Enemies { get; set; }


        public ApplicationDbContext()
        {

        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
