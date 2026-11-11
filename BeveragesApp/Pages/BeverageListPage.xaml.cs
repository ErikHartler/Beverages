using BeveragesLib.Data;
using BeveragesLib.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

using Microsoft.Extensions.Logging;
using BeveragesApp.Helpers;

namespace BeveragesApp;

public partial class BeverageListPage : ContentPage
{

    #region private members
    private static readonly ILogger<BeverageListPage> _logger = PageLogging.CreateLogger<BeverageListPage>();
    private readonly DataContext _dataContext;
    private readonly CancellationToken _cancellationToken;
    private readonly ContentPageHelper _contentPageHelper;
    #endregion
    #region constructor
    public BeverageListPage(DataContext dataContext, CancellationToken cancellationToken)
    {
        InitializeComponent();
        if (_logger.IsEnabled(LogLevel.Information)) _logger.LogInformation("{PageName} opened", nameof(BeverageListPage));
        _dataContext = dataContext;
        _cancellationToken = cancellationToken;
        LoadBeveragesAsync(_cancellationToken);
        _contentPageHelper = new ContentPageHelper(this, _dataContext, _cancellationToken);
        if (_logger.IsEnabled(LogLevel.Information)) _logger.LogInformation("{PageName} ended", nameof(BeverageListPage));
    }
    #endregion
    #region public properties
    public ObservableCollection<Beverage> Beverages { get; set; } = [];
    #endregion
    #region protected event handlers
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (_logger.IsEnabled(LogLevel.Information)) _logger.LogInformation("{PageName} appeared", GetType().Name);
        LoadBeveragesAsync(_cancellationToken);
        if (_logger.IsEnabled(LogLevel.Information)) _logger.LogInformation("{PageName} appeared ended", GetType().Name);
    }

    #endregion
    #region private event handlers

    private async  void LoadBeveragesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _dataContext.Database.EnsureCreatedAsync(cancellationToken); // Ensure the database is created before querying
            List<Beverage> beverages = await _dataContext.Beverages
                .Include(beverage => beverage.Category)
                .ToListAsync(cancellationToken);
            Beverages.Clear();
            foreach (Beverage? beverage in beverages)
            {
                Beverages.Add(beverage); // Add each retrieved drink to the ObservableCollection
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Operation failed on {PageName}", nameof(BeverageListPage));
            await DisplayAlertAsync("Fel", $"Ett fel uppstod i laddningen: {ex}", "OK"); // Display an error message if an exception occurs
        }
    }  
    
    private async void OnAddBeverageClicked(object? sender, EventArgs e)
    {
        if (_logger.IsEnabled(LogLevel.Information)) _logger.LogInformation("Executing {PageAction}", nameof(OnAddBeverageClicked));
        using var _ = new ButtonExecutionScope(sender);
        try
        {

            BeverageItemPage page = new(
                _dataContext, 
                new Beverage() 
                { 
                    Name = "", 
                    Price=0,
                    DateOfPurchase = DateTime.Now,
                    Description = "",
                    ReviewRemark =new Remark() {Id=1,Name="" }, 
                    CountryOfOrigin = new Country() { Id = 1, Name = "" }, 
                    Category = new BeverageCategory() { Id = 1, Name = "" } 
                }, 
                _cancellationToken);
            await Navigation.PushAsync(page);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Operation failed on {PageName}", nameof(BeverageListPage));
            await DisplayAlertAsync("Fel", $"Ett fel uppstod: {ex}", "OK"); // Display an error message if an exception occurs
        }
    }
    private async void OnDeleteBeverageClicked(object? sender, EventArgs e)
    {
        if (_logger.IsEnabled(LogLevel.Information)) _logger.LogInformation("Executing {PageAction}", nameof(OnDeleteBeverageClicked));
        using var _ = new ButtonExecutionScope(sender);
        var selectedBeverage = (sender as ImageButton)?.CommandParameter as Beverage;
        if (await _contentPageHelper.ConfirmDeleteAsync(selectedBeverage!, "drycken"))
        {

            try
            {
                _dataContext.Beverages.Remove(selectedBeverage!); // Remove the selected drink from the DbSet
                await _dataContext.SaveChangesAsync(); // Save changes to the database
                LoadBeveragesAsync(_cancellationToken); // Refresh the list of drinks after deletion
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Operation failed on {PageName}", nameof(BeverageListPage));
                await DisplayAlertAsync("Fel", $"Ett fel uppstod vid borttagning av drycken: {ex}", "OK"); // Display an error message if an exception occurs
            }
        }
    }
    private async void OnUpdateBeverageClicked(object? sender, EventArgs e)
    {
        if (_logger.IsEnabled(LogLevel.Information)) _logger.LogInformation("Executing {PageAction}", nameof(OnUpdateBeverageClicked));
        using var _ = new ButtonExecutionScope(sender);
        if (sender is ImageButton button && button.CommandParameter is Beverage selectedBeverage)
        {
            if (string.IsNullOrEmpty(selectedBeverage.Name))
            {
                await DisplayAlertAsync(
                    "Fel",
                    "Vänligen ange ett nytt kategorinamn.",
                    "OK"); // Display an error message if the new category name is empty
                return;
            }
            try
            {
                Beverage? beverage = await _dataContext.Beverages
                    .Include(b => b.Category)
                    .Include(b => b.CountryOfOrigin)
                    .Include(b => b.ReviewRemark)
                    .FirstOrDefaultAsync(
                        b => b.Id == selectedBeverage.Id, _cancellationToken);
                BeverageItemPage page = new(_dataContext, beverage!, _cancellationToken);
                await Navigation.PushAsync(page);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Operation failed on {PageName}", nameof(BeverageListPage));
                await DisplayAlertAsync("Fel", $"Ett fel uppstod vid uppdatering av drycken: {ex}", "OK"); // Display an error message if an exception occurs
            }
        }
        else
        {
            await DisplayAlertAsync("Fel", "Välj dryck att uppdatera.", "OK"); // Display an error message if no drink is selected
        }
    }
    #endregion
    private async void OnQuitClicked(object? sender, EventArgs e)
    {
        if (_logger.IsEnabled(LogLevel.Information)) _logger.LogInformation("Executing {PageAction}", nameof(OnQuitClicked));
        await _contentPageHelper.OnQuitClickedAsync(sender, e);
    }

    private async void GoBack(object? sender, EventArgs e)
    {
        if (_logger.IsEnabled(LogLevel.Information)) _logger.LogInformation("Executing {PageAction}", nameof(GoBack));
        using var _ = new ButtonExecutionScope(sender);
        await Navigation.PopAsync();
    }

}
