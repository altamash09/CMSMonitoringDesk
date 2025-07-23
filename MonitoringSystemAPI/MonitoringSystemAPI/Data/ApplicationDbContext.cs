using Microsoft.EntityFrameworkCore;
using MonitoringSystemAPI.Models.Entities;

namespace MonitoringSystemAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Agent> Agents { get; set; }
        public DbSet<Reviewer> Reviewers { get; set; }
        public DbSet<MonitoringStats> MonitoringStats { get; set; }
        public DbSet<HourlyData> HourlyData { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User Configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.PasswordHash).HasMaxLength(500);
                entity.Property(e => e.Permissions).HasMaxLength(1000);
            });

            // Agent Configuration
            modelBuilder.Entity<Agent>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.EstimatedHours).HasPrecision(5, 2);
                entity.Property(e => e.ActualHours).HasPrecision(5, 2);
            });

            // Reviewer Configuration
            modelBuilder.Entity<Reviewer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.EstimatedHours).HasPrecision(5, 2);
                entity.Property(e => e.ActualHours).HasPrecision(5, 2);
            });

            // MonitoringStats Configuration
            modelBuilder.Entity<MonitoringStats>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.Name, e.Date, e.IsBacklog });
            });

            // HourlyData Configuration
            modelBuilder.Entity<HourlyData>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => new { e.Date, e.Hour }).IsUnique();
            });

            // Seed Data
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Default Admin User
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Name = "System Administrator",
                    Username = "admin",
                    Email = "admin@monitoringsystem.com",
                    Phone = "+1-555-000-0001",
                    PasswordHash = "admin123", // Default password
                    Role = "admin",
                    Status = "active",
                    Department = "IT",
                    Permissions = "[\"dashboard\",\"users\",\"monitoring\",\"security\",\"analytics\",\"reports\",\"notifications\",\"settings\"]",
                    JoinDate = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            );

            // Seed Sample Agents
            var agents = new[]
            {
                new Agent { Id = 1, Name = "John Smith", Status = "online", Completed = 45, EstimatedHours = 8.5m, ActualHours = 7.2m, Rank = "Diamond", Efficiency = 95 },
                new Agent { Id = 2, Name = "Sarah Johnson", Status = "online", Completed = 52, EstimatedHours = 9.0m, ActualHours = 8.1m, Rank = "Platinum", Efficiency = 92 },
                new Agent { Id = 3, Name = "Mike Chen", Status = "idle", Completed = 38, EstimatedHours = 7.5m, ActualHours = 6.8m, Rank = "Gold", Efficiency = 88 },
                new Agent { Id = 4, Name = "Emma Davis", Status = "online", Completed = 41, EstimatedHours = 8.0m, ActualHours = 7.5m, Rank = "Diamond", Efficiency = 90 },
                new Agent { Id = 5, Name = "Alex Rodriguez", Status = "offline", Completed = 29, EstimatedHours = 6.5m, ActualHours = 5.9m, Rank = "Silver", Efficiency = 82 },
                new Agent { Id = 6, Name = "Lisa Wang", Status = "online", Completed = 47, EstimatedHours = 8.2m, ActualHours = 7.8m, Rank = "Platinum", Efficiency = 94 },
                new Agent { Id = 7, Name = "David Brown", Status = "idle", Completed = 35, EstimatedHours = 7.0m, ActualHours = 6.5m, Rank = "Gold", Efficiency = 85 },
                new Agent { Id = 8, Name = "Anna Martinez", Status = "online", Completed = 43, EstimatedHours = 8.1m, ActualHours = 7.3m, Rank = "Silver", Efficiency = 87 },
                new Agent { Id = 9, Name = "Kevin Park", Status = "online", Completed = 39, EstimatedHours = 7.8m, ActualHours = 7.0m, Rank = "Gold", Efficiency = 89 },
                new Agent { Id = 10, Name = "Rachel Green", Status = "idle", Completed = 33, EstimatedHours = 6.8m, ActualHours = 6.2m, Rank = "Silver", Efficiency = 86 },
                new Agent { Id = 11, Name = "Tom Wilson", Status = "offline", Completed = 25, EstimatedHours = 6.0m, ActualHours = 5.5m, Rank = "Bronze", Efficiency = 78 },
                new Agent { Id = 12, Name = "Jessica Lee", Status = "online", Completed = 48, EstimatedHours = 8.5m, ActualHours = 8.0m, Rank = "Platinum", Efficiency = 93 },
                new Agent { Id = 13, Name = "Mark Thompson", Status = "online", Completed = 42, EstimatedHours = 7.9m, ActualHours = 7.4m, Rank = "Diamond", Efficiency = 91 }
            };
            modelBuilder.Entity<Agent>().HasData(agents);

            // Seed Sample Reviewers
            var reviewers = new[]
            {
                new Reviewer { Id = 1, Name = "Robert Wilson", Status = "online", Completed = 28, EstimatedHours = 6.5m, ActualHours = 5.8m, Rank = "Diamond", Efficiency = 96 },
                new Reviewer { Id = 2, Name = "Jennifer Lee", Status = "online", Completed = 32, EstimatedHours = 7.0m, ActualHours = 6.2m, Rank = "Platinum", Efficiency = 93 },
                new Reviewer { Id = 3, Name = "Mark Taylor", Status = "idle", Completed = 25, EstimatedHours = 6.0m, ActualHours = 5.5m, Rank = "Gold", Efficiency = 89 },
                new Reviewer { Id = 4, Name = "Sophie Anderson", Status = "online", Completed = 30, EstimatedHours = 6.8m, ActualHours = 6.0m, Rank = "Gold", Efficiency = 91 },
                new Reviewer { Id = 5, Name = "Chris Thompson", Status = "offline", Completed = 18, EstimatedHours = 5.0m, ActualHours = 4.2m, Rank = "Bronze", Efficiency = 78 },
                new Reviewer { Id = 6, Name = "Maya Patel", Status = "online", Completed = 26, EstimatedHours = 6.2m, ActualHours = 5.7m, Rank = "Silver", Efficiency = 86 },
                new Reviewer { Id = 7, Name = "Daniel Kim", Status = "online", Completed = 35, EstimatedHours = 7.2m, ActualHours = 6.8m, Rank = "Platinum", Efficiency = 94 },
                new Reviewer { Id = 8, Name = "Amanda White", Status = "idle", Completed = 22, EstimatedHours = 5.8m, ActualHours = 5.3m, Rank = "Silver", Efficiency = 84 },
                new Reviewer { Id = 9, Name = "Steven Davis", Status = "online", Completed = 29, EstimatedHours = 6.5m, ActualHours = 6.1m, Rank = "Gold", Efficiency = 90 },
                new Reviewer { Id = 10, Name = "Laura Brown", Status = "offline", Completed = 15, EstimatedHours = 4.5m, ActualHours = 4.0m, Rank = "Bronze", Efficiency = 76 },
                new Reviewer { Id = 11, Name = "Michael Zhang", Status = "online", Completed = 33, EstimatedHours = 7.0m, ActualHours = 6.6m, Rank = "Diamond", Efficiency = 95 },
                new Reviewer { Id = 12, Name = "Nicole Garcia", Status = "online", Completed = 27, EstimatedHours = 6.3m, ActualHours = 5.9m, Rank = "Silver", Efficiency = 87 }
            };
            modelBuilder.Entity<Reviewer>().HasData(reviewers);

            // Seed Monitoring Stats
            var today = DateTime.UtcNow.Date;
            var monitoringStats = new[]
            {
                new MonitoringStats { Id = 1, Name = "Un-Monitored", Count = 45, Color = "from-red-100 to-red-200", IsMandatory = true, Date = today },
                new MonitoringStats { Id = 2, Name = "Monitoring In Process", Count = 23, Color = "from-amber-100 to-yellow-200", IsMandatory = true, Date = today },
                new MonitoringStats { Id = 3, Name = "Not Ready", Count = 12, Color = "from-gray-100 to-gray-200", IsMandatory = true, Date = today },
                new MonitoringStats { Id = 4, Name = "WFR", Count = 8, Color = "from-orange-100 to-orange-200", IsMandatory = true, Date = today },
                new MonitoringStats { Id = 5, Name = "Live", Count = 156, Color = "from-emerald-100 to-green-200", IsMandatory = true, Date = today },
                new MonitoringStats { Id = 6, Name = "Review In Process", Count = 34, Color = "from-blue-100 to-blue-200", IsMandatory = true, Date = today },
                new MonitoringStats { Id = 7, Name = "Rejected", Count = 7, Color = "from-red-200 to-red-300", IsMandatory = true, Date = today },
                new MonitoringStats { Id = 8, Name = "Tickets Opened", Count = 28, Color = "from-cyan-100 to-teal-200", IsMandatory = true, Date = today },
                new MonitoringStats { Id = 9, Name = "DVR Down", Count = 3, Color = "from-purple-100 to-purple-200", IsMandatory = false, Date = today },
                new MonitoringStats { Id = 10, Name = "Archive Datalost", Count = 2, Color = "from-pink-100 to-rose-200", IsMandatory = false, Date = today },
                new MonitoringStats { Id = 11, Name = "On-Holiday", Count = 15, Color = "from-indigo-100 to-indigo-200", IsMandatory = false, Date = today }
            };
            modelBuilder.Entity<MonitoringStats>().HasData(monitoringStats);

            // Seed Hourly Data (24 hours)
            var hourlyData = Enumerable.Range(0, 24).Select(hour => new HourlyData
            {
                Id = hour + 1,
                Hour = hour,
                Completed = Random.Shared.Next(10, 55),
                Percentage = Random.Shared.Next(75, 98),
                Date = today
            }).ToArray();

            modelBuilder.Entity<HourlyData>().HasData(hourlyData);
        }
    }

}
