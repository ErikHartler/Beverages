using BeveragesLib.Data;
using BeveragesLib.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BeveragesApp.Pages;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
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
                string dbPath = Path.Combine(FileSystem.AppDataDirectory, "beverages.db");
                //File.Delete(dbPath); // Delete the existing database file to ensure a fresh start
                options.UseSqlite($"Data Source={dbPath}");
            }); 
#if DEBUG
        builder.Logging.SetMinimumLevel(LogLevel.Debug);
        builder.Logging.AddDebug();
#endif
        MauiApp app = builder.Build();
        using (IServiceScope scope = app.Services.CreateScope())
        {
            DataContext dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
            dbContext.Database.EnsureCreated(); // Ensure the database is created when the application starts
            bool anySeeding = false;
            if (!dbContext.BeverageCategories.Any())
            {
                // Add initial data if the database is empty
                dbContext.BeverageCategories.AddRange(BeverageCategory.Seed());
                anySeeding = true;
            }
            if (!dbContext.Countries.Any())
            {
                dbContext.Countries.AddRange(Country.Seed());
                anySeeding = true;
            }
            if (!dbContext.Remarks.Any())
            {
                dbContext.Remarks.AddRange(Remark.Seed());
                anySeeding = true;
            }
            if (anySeeding)
            {
                dbContext.SaveChanges();
            }
        }
        return app;
    }
}

