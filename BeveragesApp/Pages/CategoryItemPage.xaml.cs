using BeveragesLib.Data;
using BeveragesLib.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace BeveragesApp;

public partial class CategoryItemPage : ContentPage
{
    #region private members
    private readonly DataContext _dataContext;
    private readonly CancellationToken _cancellationToken;
    private readonly ContentPageHelper _contentPageHelper;

    private readonly BeverageCategory _beverageCategory; // Property to hold the selected beverage category  
    #endregion
    #region constructor
    public CategoryItemPage(DataContext dataContext, BeverageCategory beverageCategory, CancellationToken cancellationToken)
    {
        InitializeComponent();
        _dataContext = dataContext; // Assign the injected DataContext instance to the private field
        _cancellationToken = cancellationToken;
        _contentPageHelper = new ContentPageHelper(this, _dataContext, _cancellationToken);
        _beverageCategory = beverageCategory;
        CategoryNameEntry.Text = beverageCategory.Name;
        CategoryIdEntry.Text = beverageCategory.Id.ToString();
    }
    #endregion
    #region public properties

    #endregion

    #region private event handlers

  
    private async void OnDeleteBeverageClicked(object? sender, EventArgs e)
    {
        using var _ = new ButtonExecutionScope(sender);
        if (await _contentPageHelper.ConfirmDeleteAsync(_beverageCategory, "kategorin"))
        {
            try
            {
                _dataContext.BeverageCategories.Remove(_beverageCategory!); // Remove the selected category from the DbSet
                await _dataContext.SaveChangesAsync(); // Save changes to the database
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Fel", $"Ett fel uppstod vid borttagning av kategorin: {ex}", "OK"); // Display an error message if an exception occurs
            }
        }
    }
    private async void OnUpdateBeverageClicked(object? sender, EventArgs e)
    {
        using var _ = new ButtonExecutionScope(sender);
        if (sender is ImageButton)
        {
            if (string.IsNullOrEmpty(CategoryNameEntry.Text))
            {
                await DisplayAlertAsync(
                    "Fel",
                    "ange kategorins namn.",
                    "OK"); // Display an error message if the new category name is empty
                return;
            }
            try
            {
                int id = int.Parse(CategoryIdEntry.Text.Trim());
                _beverageCategory.Name = CategoryNameEntry.Text.Trim(); 
                if (id >0)
                {
                    _beverageCategory.Id = id;
                    _dataContext.BeverageCategories.Update(_beverageCategory);
                }
                else
                {
                    _dataContext.BeverageCategories.Add(_beverageCategory);
               }
                if (await _dataContext.BeverageCategories.AnyAsync(x => x.Name == CategoryNameEntry.Text.Trim() && x.Id != _beverageCategory.Id))
                {
                    await DisplayAlertAsync("Fel", $"Namnet {CategoryNameEntry.Text.Trim()} är redan registrerat.", "OK");
                    return;
                }
                try
                {
                    await _dataContext.SaveChangesAsync();
                }
                catch (DbUpdateException dbEx) when (dbEx.InnerException is SqliteException sqliteEx && sqliteEx.SqliteErrorCode == 19)
                {
                    await DisplayAlertAsync("Fel", $"Gick inte att spara, {dbEx}", "OK");
                    return;
                };

                var category = await _dataContext.BeverageCategories.FirstAsync<BeverageCategory>(x => x.Name == CategoryNameEntry.Text.Trim(), _cancellationToken);
                CategoryIdEntry.Text = category.Id.ToString();

            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Fel", $"Ett fel uppstod vid uppdatering av kategorin: {ex}", "OK"); // Display an error message if an exception occurs
            }
        }
        else
        {
            await DisplayAlertAsync("Fel", "Välj kategori att uppdatera.", "OK"); // Display an error message if no drink is selected
        }
    }
    #endregion
    private async void OnQuitClicked(object? sender, EventArgs e)
    {
        await _contentPageHelper.OnQuitClickedAsync(sender, e);
    }

    private async void GoBack(object? sender, EventArgs e)
    {
        using var _ = new ButtonExecutionScope(sender);
        await Navigation.PopAsync();
    }

}
