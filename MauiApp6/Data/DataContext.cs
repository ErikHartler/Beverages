
using DrinkManagement.Models;
using Microsoft.EntityFrameworkCore;


namespace DrinkManagement.Data;

public partial class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
{
    public DbSet<Beverage> Drinks { get; set; }
}
