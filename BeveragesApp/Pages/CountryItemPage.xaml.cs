using BeveragesLib.Data;
using BeveragesLib.Models;

namespace BeveragesApp;

public partial class CountryItemPage : ContentPage
{
    #region private members
    private readonly DataContext _dataContext;
    private readonly CancellationToken _cancellationToken;
    private readonly ContentPageHelper _contentPageHelper;

    private readonly Country _country; // Property to hold the selected country
    #endregion
    #region constructor
    public CountryItemPage(DataContext dataContext, Country country, CancellationToken cancellationToken)
    {
        InitializeComponent();
        _dataContext = dataContext; // Assign the injected DataContext instance to the private field
        _cancellationToken = cancellationToken;
        _contentPageHelper = new ContentPageHelper(this, _dataContext, _cancellationToken);
 
        _country = country;
        BindingContext = this; //MVVM pattern: The BindingContext is set to the BeverageListPage instance itself, allowing data binding between the UI and the code-behind.
        CountryNameEntry.Text = _country.Name;
        CountryIdEntry.Text = _country.Id.ToString();
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
    private async void OnDeleteCountryClicked(object? sender, EventArgs e)
    {
        if (int.Parse(CountryIdEntry.Text.Trim()) == 0) return;
        if (sender is Button button && button.CommandParameter is Country selectedCountry )
        {
            bool confirmDelete = await DisplayAlertAsync("Bekräfta borttagning", $"Är du säker på att du vill ta bort landet '{selectedCountry.Name}'?", "Yes", "No"); // Ask for confirmation before deleting
            if (!confirmDelete)
            {
                return; // Exit the method if the user cancels the deletion
            }

            try
            {
                _dataContext.Countries.Remove(selectedCountry); // Remove the selected country from the DbSet
                await _dataContext.SaveChangesAsync(); // Save changes to the database
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Fel", $"Ett fel uppstod vid borttagning av landet: {ex}", "OK"); // Display an error message if an exception occurs
            }
        }
        else
        {
            await DisplayAlertAsync("Fel", "Vänligen välj en dryck att ta bort.", "OK"); // Display an error message if no drink is selected
        }
    }
    private async void OnUpdateCountryClicked(object? sender, EventArgs e)
    {
        if (sender is Button)
        {
            if (string.IsNullOrEmpty(CountryNameEntry.Text))
            {
                await DisplayAlertAsync(
                    "Fel",
                    "ange landets namn.",
                    "OK"); // Display an error message if the new country name is empty
                return;
            }
            try
            {
                int id = int.Parse(CountryIdEntry.Text.Trim());
                _country.Name = CountryNameEntry.Text.Trim(); 
                if (id >0)
                {
                    _country.Id = id;
                    _dataContext.Countries.Update(_country);
                }
                else
                {
                    _dataContext.Countries.Add(_country);
                }
                await _dataContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Fel", $"Ett fel uppstod vid uppdatering av landet: {ex}", "OK"); // Display an error message if an exception occurs
            }
        }
        else
        {
            await DisplayAlertAsync("Fel", "Välj land att uppdatera.", "OK"); // Display an error message if no country is selected
        }
    }
    #endregion

}
