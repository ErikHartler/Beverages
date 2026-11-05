using BeveragesLib.Data;
using BeveragesLib.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace BeveragesApp;

public partial class CategoryListPage : ContentPage
{
    #region private members
    private readonly DataContext _dataContext;
    private readonly CancellationToken _cancellationToken;
    private readonly ContentPageHelper _contentPageHelper;
    #endregion

    #region constructor
    public CategoryListPage(DataContext dataContext, CancellationToken cancellationToken)
    {
        InitializeComponent();
        _dataContext = dataContext;
        _cancellationToken = cancellationToken;
        _contentPageHelper = new ContentPageHelper(this, _dataContext, _cancellationToken);
 
        Categories = [];
        BindingContext = this; 

        LoadCategories(CancellationToken.None); 
    }
    #endregion
    #region public  properties
    public ObservableCollection<BeverageCategory> Categories { get; set; }
    #endregion

    #region private methods
    private async void OnQuitClicked(object? sender, EventArgs e)
    {
        _contentPageHelper.OnQuitClicked(sender, e);
    }
    private async void LoadCategories(CancellationToken cancellationToken)
    {
        try
        {
            Categories.Clear();  

            await _dataContext.Database.EnsureCreatedAsync(cancellationToken); // Ensure the database is created before querying


            List<BeverageCategory> beverageCategories = await AsyncEnumerable.ToListAsync(_dataContext.BeverageCategories.AsAsyncEnumerable(), cancellationToken); // Asynchronously retrieve the list of drinks from the database
            foreach (BeverageCategory? category in beverageCategories)
            {
                Categories.Add(category); 
            }

        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Fel", $"Ett fel uppstod i laddningen: {ex}", "OK"); // Display an error message if an exception occurs
        }
    }
    #endregion
    #region private event handlers
    private async void OnAddCategoryClicked(object? sender, EventArgs e)
    {
        try
        {
            CategoryItemPage page = new(_dataContext, new BeverageCategory() { Name=""}, _cancellationToken);
            await Navigation.PushAsync(page);
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Fel", $"Ett fel uppstod: {ex}", "OK"); // Display an error message if an exception occurs
        }
    }

    private async void OnDeleteCategoryClicked(object? sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is BeverageCategory selectedCategory)
        {
            bool confirmDelete = await DisplayAlertAsync("Bekräfta borttagning", $"Är du säker på att du vill ta bort kategorin '{selectedCategory.Name}'?", "Yes", "No"); // Ask for confirmation before deleting
            if (!confirmDelete)
            {
                return; // Exit the method if the user cancels the deletion
            }

            try
            {
                _dataContext.BeverageCategories.Remove(selectedCategory); // Remove the selected drink from the DbSet
                await _dataContext.SaveChangesAsync(); // Save changes to the database
                Categories.Remove(selectedCategory); // Remove the selected drink from the ObservableCollection to update the UI
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Fel", $"Ett fel uppstod vid borttagning av kategorin: {ex}", "OK"); // Display an error message if an exception occurs
            }
        }
        else
        {
            await DisplayAlertAsync("Fel", "Vänligen välj en kategori att ta bort.", "OK"); // Display an error message if no drink is selected
        }
    }
    private async void OnUpdateCategoryClicked(object? sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is BeverageCategory selectedCategory)
        {
            if (string.IsNullOrEmpty(selectedCategory.Name))
            {
                await DisplayAlertAsync(
                    "Fel",
                    "Vänligen ange ett nytt kategorinamn.",
                    "OK"); // Display an error message if the new category name is empty
                return;
            }
            try
            {
                BeverageCategory? beverageCategory = await _dataContext.BeverageCategories.FirstOrDefaultAsync(
                        c => c.Id == selectedCategory.Id, CancellationToken.None
                    ); 
                CategoryItemPage page = new(_dataContext, beverageCategory! , _cancellationToken);
                await Navigation.PushAsync(page);
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Fel", $"Ett fel uppstod vid uppdatering av kategorin: {ex}", "OK"); // Display an error message if an exception occurs
            }
        }
        else
        {
            await DisplayAlertAsync("Fel", "Välj kategori att uppdatera.", "OK"); // Display an error message if no category is selected
        }
    }
    #endregion

}
