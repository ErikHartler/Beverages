using BeveragesLib.Data;
using BeveragesLib.Models;

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
        BindingContext = this; //MVVM pattern: The BindingContext is set to the BeverageListPage instance itself, allowing data binding between the UI and the code-behind.
        CategoryNameEntry.Text = beverageCategory.Name;
        CategoryIdEntry.Text = beverageCategory.Id.ToString();
    }
    #endregion
    #region public properties

    #endregion

    #region private methods
    #endregion
    #region private event handlers

    private async void OnQuitClicked(object? sender, EventArgs e)
    {
        _contentPageHelper.OnQuitClicked(sender, e);
    }
    private async void OnDeleteBeverageClicked(object? sender, EventArgs e)
    {
        if (int.Parse(CategoryIdEntry.Text.Trim()) == 0) return;
        if (sender is Button button && button.CommandParameter is Beverage selectedBeverage )
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
        if (sender is Button)
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
                await _dataContext.SaveChangesAsync(); 
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

}
