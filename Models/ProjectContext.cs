using Microsoft.EntityFrameworkCore;
 
namespace DefaultProject.Models
{
    public class ProjectContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public ProjectContext(DbContextOptions<ProjectContext> options) : base(options) { }

        public DbSet<User> users { get; set; }
        public DbSet<Mimic> mimics { get; set; }
        public DbSet<Item> items { get; set; }

    }
}