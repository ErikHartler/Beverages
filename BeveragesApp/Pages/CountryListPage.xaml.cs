using BeveragesLib.Data;
using BeveragesLib.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

using Microsoft.Extensions.Logging;

namespace BeveragesApp;

public partial class CountryListPage : ContentPage
{
    private readonly ILogger<CountryListPage> _logger = PageLogging.CreateLogger<CountryListPage>();
    #region private members
    private readonly DataContext _dataContext;
    private readonly CancellationToken _cancellationToken;
    private readonly ContentPageHelper _contentPageHelper;

    #endregion
    #region constructor
    public CountryListPage(DataContext dataContext, CancellationToken cancellationToken)
    {
        InitializeComponent();
        _logger.LogInformation("{PageName} opened", nameof(CountryListPage));
        _dataContext = dataContext;
        _cancellationToken = cancellationToken;
        _contentPageHelper = new ContentPageHelper(this, _dataContext, _cancellationToken);
        LoadCountries(cancellationToken);
        _logger.LogInformation("{PageName} ended", nameof(CountryListPage));
    }
    #endregion
    #region public  properties
    public ObservableCollection<Country> Countries { get; set; } = [];
    #endregion

    #region private event handlers

    private async void LoadCountries(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Executing {PageAction}", nameof(LoadCountries));
        try
        {
            Countries.Clear();  

            await _dataContext.Database.EnsureCreatedAsync(cancellationToken); // Ensure the database is created before querying


            List<Country> countries = await AsyncEnumerable.ToListAsync(_dataContext.Countries.AsAsyncEnumerable(), cancellationToken); // Asynchronously retrieve the list of countries from the database
            foreach (Country? country in countries)
            {
                Countries.Add(country); 
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Operation failed on {PageName}", nameof(CountryListPage));
            await DisplayAlertAsync("Fel", $"Ett fel uppstod i laddningen: {ex}", "OK"); // Display an error message if an exception occurs
        }
    }
    
    private async void OnAddCountryClicked(object? sender, EventArgs e)
    {
        _logger.LogDebug("Executing {PageAction}", nameof(OnAddCountryClicked));
        using var _ = new ButtonExecutionScope(sender);
        try
        {
            CountryItemPage page = new(_dataContext, new Country() { Name=""}, _cancellationToken);
            await Navigation.PushAsync(page);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Operation failed on {PageName}", nameof(CountryListPage));
            await DisplayAlertAsync("Fel", $"Ett fel uppstod: {ex}", "OK"); // Display an error message if an exception occurs
        }
    }
    
    private async void OnDeleteCountryClicked(object? sender, EventArgs e)
    {
        _logger.LogDebug("Executing {PageAction}", nameof(OnDeleteCountryClicked));
        using var _ = new ButtonExecutionScope(sender);
        var selectedCountry = (sender as ImageButton)?.CommandParameter as Country;
        if (await _contentPageHelper.ConfirmDeleteAsync(selectedCountry!, "landet"))
        {
            

            try
            {
                _dataContext.Countries.Remove(selectedCountry!); // Remove the selected country from the DbSet
                await _dataContext.SaveChangesAsync(); // Save changes to the database
                Countries.Remove(selectedCountry!); // Remove the selected country from the ObservableCollection to update the UI
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Operation failed on {PageName}", nameof(CountryListPage));
                await DisplayAlertAsync("Fel", $"Ett fel uppstod vid borttagning av landet: {ex}", "OK"); // Display an error message if an exception occurs
            }
        }
        else
        {
            await DisplayAlertAsync("Fel", "Vänligen välj ett land att ta bort.", "OK"); // Display an error message if no country is selected
        }
    }
    private async void OnUpdateCountryClicked(object? sender, EventArgs e)
    {
        _logger.LogDebug("Executing {PageAction}", nameof(OnUpdateCountryClicked));
        using var _ = new ButtonExecutionScope(sender);
        if (sender is ImageButton button && button.CommandParameter is Country selectedCountry)
        {
            if (string.IsNullOrEmpty(selectedCountry.Name))
            {
                await DisplayAlertAsync(
                    "Fel",
                    "Vänligen ange ett nytt landsnamn.",
                    "OK"); // Display an error message if the new country name is empty
                return;
            }
            try
            {
                Country? country = await _dataContext.Countries.FirstOrDefaultAsync(
                        c => c.Id == selectedCountry.Id, _cancellationToken
                    ); 
                CountryItemPage page = new(_dataContext, country! , _cancellationToken);
                await Navigation.PushAsync(page);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Operation failed on {PageName}", nameof(CountryListPage));
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
