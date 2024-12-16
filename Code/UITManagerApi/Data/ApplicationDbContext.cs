using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UITManagerApi.Models;

namespace UITManagerApi.Data;
public class ApplicationDbContext : DbContext {
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) {
    }
    
    public DbSet<Machine> Machines { get; set; }
    public DbSet<Information> Components { get; set; }

    protected override void OnModelCreating(ModelBuilder builder) {
        base.OnModelCreating(builder);
        
        builder.Entity<Models.Information>()
            .HasDiscriminator<string>("ComponentType")
            .HasValue<Value>("Leaf")
            .HasValue<Component>("Composite");
        
        builder.Entity<Information>()
            .HasMany(c => c.Children)
            .WithOne(c => c.Parent)
            .HasForeignKey(c => c.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Machine>()
            .HasMany(m => m.Informations)
            .WithOne(c => c.Machine)
            .HasForeignKey(c => c.MachinesId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}