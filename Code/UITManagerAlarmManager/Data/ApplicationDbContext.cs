using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UITManagerWebServer.Models;

namespace UITManagerWebServer.Data {
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser> {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) {
        }
        
        public DbSet<Machine> Machines { get; set; }
        
        public DbSet<Alarm> Alarms { get; set; }

        public DbSet<NormGroup> NormGroups { get; set; }

        public DbSet<Norm> Norms { get; set; }

        public DbSet<AlarmStatusHistory> AlarmHistories { get; set; }

        public DbSet<AlarmStatusType> AlarmStatusTypes { get; set; }
        
        public DbSet<Models.Information> Components { get; set; }
        public DbSet<Value> Leafs { get; set; }
        public DbSet<Component> Composites { get; set; }
        
        public DbSet<InformationName> InformationNames { get; set; } 

        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);

            builder.Entity<Alarm>()
                .HasOne(a => a.Machine)
                .WithMany(m => m.Alarms)
                .HasForeignKey(a => a.MachineId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Norm>()
                .HasOne(n => n.NormGroup)
                .WithMany(ng => ng.Norms)
                .HasForeignKey(n => n.NormGroupId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<AlarmStatusHistory>()
                .HasOne(ash => ash.Alarm)
                .WithMany(a => a.AlarmHistories)
                .HasForeignKey(ash => ash.AlarmId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<AlarmStatusHistory>()
                .HasOne(ash => ash.StatusType)
                .WithMany(st => st.AlarmStatusHistories)
                .HasForeignKey(ash => ash.StatusTypeId)
                .OnDelete(DeleteBehavior.SetNull);
            
            builder.Entity<Models.Information>()
                .HasDiscriminator<string>("ComponentType")
                .HasValue<Value>("Leaf")
                .HasValue<Component>("Composite");
            
            builder.Entity<Models.Information>()
                .HasMany(c => c.Children)
                .WithOne(c => c.Parent)
                .HasForeignKey(c => c.ParentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Machine>()
                .HasMany(m => m.Informations)
                .WithOne(c => c.Machine)
                .HasForeignKey(c => c.MachinesId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<InformationName>()
                .HasMany(m => m.SubInformationNames)
                .WithOne()
                .HasForeignKey("ParentId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Norm>()
                .HasOne(n => n.InformationName)
                .WithMany()
                .HasForeignKey(n => n.InformationNameId);
        }
    }
}