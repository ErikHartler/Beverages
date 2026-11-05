using BeveragesLib.Data;
using BeveragesLib.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace BeveragesApp;

public partial class BeverageListPage : ContentPage
{
    #region private members
    private readonly DataContext _dataContext;
    private readonly CancellationToken _cancellationToken;
    private readonly ContentPageHelper _contentPageHelper;
    #endregion
    #region constructor
    public BeverageListPage(DataContext dataContext, CancellationToken cancellationToken)
    {
        InitializeComponent();
        _dataContext = dataContext;
        _cancellationToken = cancellationToken;
        _contentPageHelper = new ContentPageHelper(this, _dataContext, _cancellationToken);
 
        Beverages = [];
        BindingContext = this; 
    }
    #endregion
    #region public properties
    public ObservableCollection<Beverage> Beverages { get; set; }
    #endregion

    #region private event handlers
    private async void OnQuitClicked(object? sender, EventArgs e)
    {
        _contentPageHelper.OnQuitClicked(sender, e);
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadBeveragesAsync(CancellationToken.None);
    }

    private async Task LoadBeveragesAsync(CancellationToken cancellationToken)
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
            await DisplayAlertAsync("Fel", $"Ett fel uppstod i laddningen: {ex}", "OK"); // Display an error message if an exception occurs
        }
    }

    private async void OnRefreshClicked(object? sender, EventArgs e)
    {
        await LoadBeveragesAsync(CancellationToken.None);
    }
     
    
    private async void OnAddBeverageClicked(object? sender, EventArgs e)
    {
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
            await DisplayAlertAsync("Fel", $"Ett fel uppstod: {ex}", "OK"); // Display an error message if an exception occurs
        }
    }
    private async void OnDeleteBeverageClicked(object? sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Beverage selectedBeverage)
        {
            bool confirmDelete = await DisplayAlertAsync("Bekräfta borttagning", $"Är du säker på att du vill ta bort drycken '{selectedBeverage.Name}'?", "Yes", "No"); // Ask for confirmation before deleting
            if (!confirmDelete)
            {
                return; // Exit the method if the user cancels the deletion
            }

            try
            {
                _dataContext.Beverages.Remove(selectedBeverage); // Remove the selected drink from the DbSet
                await _dataContext.SaveChangesAsync(); // Save changes to the database
                await LoadBeveragesAsync(CancellationToken.None); // Refresh the list of drinks after deletion
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Fel", $"Ett fel uppstod vid borttagning av drycken: {ex}", "OK"); // Display an error message if an exception occurs
            }
        }
        else
        {
            await DisplayAlertAsync("Fel", "Vänligen välj en dryck att ta bort.", "OK"); // Display an error message if no drink is selected
        }
    }
    private async void OnUpdateBeverageClicked(object? sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Beverage selectedBeverage)
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
                        b => b.Id == selectedBeverage.Id, CancellationToken.None);
                BeverageItemPage page = new(_dataContext, beverage!, _cancellationToken);
                await Navigation.PushAsync(page);
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Fel", $"Ett fel uppstod vid uppdatering av drycken: {ex}", "OK"); // Display an error message if an exception occurs
            }
        }
        else
        {
            await DisplayAlertAsync("Fel", "Välj dryck att uppdatera.", "OK"); // Display an error message if no drink is selected
        }
    }
    #endregion

}
