using BeveragesLib.Data;
using BeveragesLib.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

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
        CountryNameEntry.Text = _country.Name;
        CountryIdEntry.Text = _country.Id.ToString();
    }
    #endregion

    #region private event handlers
    private async void OnDeleteCountryClicked(object? sender, EventArgs e)
    {
        using var _ = new ButtonExecutionScope(sender);
        if (await _contentPageHelper.ConfirmDeleteAsync(_country, "landet"))
        {
            try
            {
                _dataContext.Countries.Remove(_country); // Remove the selected country from the DbSet
                await _dataContext.SaveChangesAsync(); // Save changes to the database
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Fel", $"Ett fel uppstod vid borttagning av landet: {ex}", "OK"); // Display an error message if an exception occurs
            }
        }
        
    }
    private async void OnUpdateCountryClicked(object? sender, EventArgs e)
    {
        using var _ = new ButtonExecutionScope(sender);
        if (sender is ImageButton)
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
                if (await _dataContext.Countries.AnyAsync(x => x.Name == CountryNameEntry.Text.Trim() && x.Id != _country.Id))
                {
                    await DisplayAlertAsync("Fel", $"Namnet {CountryNameEntry.Text.Trim()} är redan registrerat.", "OK");
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

                var country = await _dataContext.Countries.FirstAsync(x => x.Name == CountryNameEntry.Text.Trim(), _cancellationToken);
                CountryIdEntry.Text = country.Id.ToString();
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
