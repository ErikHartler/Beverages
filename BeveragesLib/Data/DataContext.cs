using BeveragesLib.Models;
using Microsoft.EntityFrameworkCore;


namespace BeveragesLib.Data;

public partial class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<BeverageCategory>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("BeverageCategories");
            entity.Property(e => e.Id)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<Beverage>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("Beverages");
            entity.Property(e => e.Id)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("Countries");
            entity.Property(e => e.Id)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();
        });
        modelBuilder.Entity<Remark>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("Remarks");
            entity.Property(e => e.Id)
                .HasColumnName("Id")
                .ValueGeneratedOnAdd();
        });
        base.OnModelCreating(modelBuilder);

    }
    public DbSet<BeverageCategory> BeverageCategories { get; set; }
    public DbSet<Beverage> Beverages { get; set; }
    public DbSet<Country> Countries { get; set; }
    public DbSet<Remark> Remarks{ get; set; }

}
