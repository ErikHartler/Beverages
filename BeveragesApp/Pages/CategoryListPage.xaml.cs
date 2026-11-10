using BeveragesLib.Data;
using BeveragesLib.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

using Microsoft.Extensions.Logging;

namespace BeveragesApp;

public partial class CategoryListPage : ContentPage
{
    private readonly ILogger<CategoryListPage> _logger = PageLogging.CreateLogger<CategoryListPage>();
    #region private members
    private readonly DataContext _dataContext;
    private readonly CancellationToken _cancellationToken;
    private readonly ContentPageHelper _contentPageHelper;
    #endregion

    #region constructor
    public CategoryListPage(DataContext dataContext, CancellationToken cancellationToken)
    {
        InitializeComponent();
        _logger.LogInformation("{PageName} opened", nameof(CategoryListPage));
        _dataContext = dataContext;
        _cancellationToken = cancellationToken;
        _contentPageHelper = new ContentPageHelper(this, _dataContext, _cancellationToken);
        _logger.LogInformation("{PageName} ended", nameof(CategoryListPage));
    }
    #endregion
    #region public  properties
    public ObservableCollection<BeverageCategory> Categories { get; set; } = [];
    #endregion
    #region protected event handlers
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _logger.LogDebug("{PageName} appeared", GetType().Name);
        LoadCategoriesAsync(_cancellationToken);
    }

    #endregion

    #region private event handlers
    private async void LoadCategoriesAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Executing {PageAction}", nameof(LoadCategoriesAsync));
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
            _logger.LogError(ex, "Operation failed on {PageName}", nameof(CategoryListPage));
            await DisplayAlertAsync("Fel", $"Ett fel uppstod i laddningen: {ex}", "OK"); // Display an error message if an exception occurs
        }
    }
    
    private async void OnAddCategoryClicked(object? sender, EventArgs e)
    {
        _logger.LogDebug("Executing {PageAction}", nameof(OnAddCategoryClicked));
        using var _ = new ButtonExecutionScope(sender);
        try
        {
            CategoryItemPage page = new(_dataContext, new BeverageCategory() { Name=""}, _cancellationToken);
            await Navigation.PushAsync(page);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Operation failed on {PageName}", nameof(CategoryListPage));
            await DisplayAlertAsync("Fel", $"Ett fel uppstod: {ex}", "OK"); // Display an error message if an exception occurs
        }
    }

    private async void OnDeleteCategoryClicked(object? sender, EventArgs e)
    {
        _logger.LogDebug("Executing {PageAction}", nameof(OnDeleteCategoryClicked));
        using var _ = new ButtonExecutionScope(sender);
        var selectedCategory = (sender as ImageButton)?.CommandParameter as BeverageCategory;
        if (await _contentPageHelper.ConfirmDeleteAsync(selectedCategory!, "kategorin"))
        {
         
            try
            {
                _dataContext.BeverageCategories.Remove(selectedCategory!); // Remove the selected drink from the DbSet
                await _dataContext.SaveChangesAsync(); // Save changes to the database
                Categories.Remove(selectedCategory!); // Remove the selected drink from the ObservableCollection to update the UI
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Operation failed on {PageName}", nameof(CategoryListPage));
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
        _logger.LogDebug("Executing {PageAction}", nameof(OnUpdateCategoryClicked));
        using var _ = new ButtonExecutionScope(sender);
        if (sender is ImageButton button && button.CommandParameter is BeverageCategory selectedCategory)
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
                        c => c.Id == selectedCategory.Id, _cancellationToken
                    ); 
                CategoryItemPage page = new(_dataContext, beverageCategory! , _cancellationToken);
                await Navigation.PushAsync(page);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Operation failed on {PageName}", nameof(CategoryListPage));
                await DisplayAlertAsync("Fel", $"Ett fel uppstod vid uppdatering av kategorin: {ex}", "OK"); // Display an error message if an exception occurs
            }
        }
        else
        {
            await DisplayAlertAsync("Fel", "Välj kategori att uppdatera.", "OK"); // Display an error message if no category is selected
        }
    }
    #endregion
    private async void OnQuitClicked(object? sender, EventArgs e)
    {
        _logger.LogDebug("Executing {PageAction}", nameof(OnQuitClicked));
        await _contentPageHelper.OnQuitClickedAsync(sender, e);
    }

    private async void GoBack(object? sender, EventArgs e)
    {
        _logger.LogDebug("Executing {PageAction}", nameof(GoBack));
        using var _ = new ButtonExecutionScope(sender);
        await Navigation.PopAsync();
    }

}
