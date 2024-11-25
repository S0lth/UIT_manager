using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UITManagerWebServer.Models;

namespace UITManagerWebServer.Data {
    public class ApplicationDbContext : IdentityDbContext<IdentityUser> {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) {
        }

        public DbSet<Machine> Machines { get; set; }

        public DbSet<Note> Notes { get; set; }

        public DbSet<Alarm> Alarms { get; set; }

        public DbSet<NormGroup> NormGroups { get; set; }

        public DbSet<Norm> Norms { get; set; }

        public DbSet<AlarmStatusHistory> AlarmHistories { get; set; }

        public DbSet<AlarmStatusType> AlarmStatusTypes { get; set; }

        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);

            builder.Entity<Alarm>()
                .HasOne(a => a.Machine)
                .WithMany(m => m.Alarms)
                .HasForeignKey(a => a.MachineId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Norm>()
                .HasOne(n => n.NormGroup)
                .WithMany(ng => ng.Norms)
                .HasForeignKey(n => n.NormGroupId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Alarm>()
                .HasOne(a => a.NormGroup)
                .WithMany()
                .HasForeignKey(a => a.NormGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Alarm>()
                .HasMany(a => a.AlarmHistories)
                .WithOne(h => h.Alarm)             
                .HasForeignKey(h => h.AlarmId) 
                .OnDelete(DeleteBehavior.Cascade); 

            builder.Entity<AlarmStatusHistory>()
                .HasOne(a => a.Modifier)
                .WithMany()
                .HasForeignKey(a => a.ModifierId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<AlarmStatusHistory>()
                .HasOne(a => a.StatusType)
                .WithMany()
                .HasForeignKey(a => a.StatusTypeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}