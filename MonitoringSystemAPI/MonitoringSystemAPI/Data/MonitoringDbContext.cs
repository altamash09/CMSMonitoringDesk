using Microsoft.EntityFrameworkCore;

namespace MonitoringAPI.Data
{
    public class MonitoringDbContext : DbContext
    {
        public MonitoringDbContext(DbContextOptions<MonitoringDbContext> options) : base(options) { }

        // No DbSet properties needed since we're using stored procedures directly
        // This context is only used for connection management and raw SQL execution

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // No entity configurations needed for stored procedure approach
            base.OnModelCreating(modelBuilder);
        }
    }
}