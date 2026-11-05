using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using DrinkManagement;
using DrinkManagement.Data; // Add this if DataContext is in MauiApp6 namespace 
using DrinkManagement.Models;
 
namespace DrinkManagement;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder 
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });
        builder.Services.AddDbContext<DataContext>(
            options =>
        {
            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "drinks.db");
            options.UseSqlite($"Data Source={dbPath}");
        }); //Dependency injection for the DataContext class, which allows it to be used throughout the application.
#if DEBUG
        builder.Logging.AddDebug();
#endif
        var app = builder.Build();
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            dbContext.Database.EnsureCreated(); // Ensure the database is created when the application starts

            if (!dbContext.Drinks.Any())
            {
                // Add initial data if the database is empty
                dbContext.Drinks.AddRange(
                    new Beverage { Name = "Coffee" },
                    new Beverage { Name = "Tea" }
                );
                dbContext.SaveChanges();
            }
        }
        return app;
    }
}
