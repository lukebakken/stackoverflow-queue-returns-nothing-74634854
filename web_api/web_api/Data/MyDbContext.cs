using Microsoft.EntityFrameworkCore;
using web_api.Models;

namespace web_api.Data
{
    public class MyDbContext: DbContext
    {
        public MyDbContext(DbContextOptions options): base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Link>()
                .HasKey(nameof(Link.client_id), nameof(Link.device_id));
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Devices> Devices { get; set; }
        public DbSet<Link> Links { get; set; }

        public DbSet<Timestamp> Timestamps { get; set; }
    }
}
