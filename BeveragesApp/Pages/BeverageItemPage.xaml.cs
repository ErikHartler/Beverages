using BeveragesLib.Data;
using BeveragesLib.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace BeveragesApp;

public partial class BeverageItemPage : ContentPage
{
    #region private members
    private readonly DataContext _dataContext;
    private readonly CancellationToken _cancellationToken;
    private readonly ContentPageHelper _contentPageHelper;

    private readonly  Beverage _beverage;
    #endregion
    #region constructor


    public BeverageItemPage(DataContext dataContext, Beverage beverage, CancellationToken cancellationToken)
    {
        InitializeComponent();
        _dataContext = dataContext;
        _cancellationToken = cancellationToken;
        _beverage = beverage;
        _contentPageHelper = new ContentPageHelper(this, _dataContext, _cancellationToken); 
        BeverageIdEntry.Text = beverage.Id.ToString();
        BeverageNameEntry.Text = beverage.Name;
        BeveragePriceEntry.Text = beverage.Price.ToString();    
        BeverageDescriptionEntry.Text = beverage.Description;
        if (beverage.PhotoImage is { Length: > 0 } imageBytes)
        {
            CapturedPhotoImage.Source = ImageSource.FromStream(() => new MemoryStream(imageBytes));
        }
    }
    #endregion
    #region public properties

    #endregion
    #region protected methods

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadCategoriesAsync(_cancellationToken);
        await LoadCountriesAsync(_cancellationToken);
        await LoadRemarksAsync(_cancellationToken);
    }
    #endregion
    #region private event handlers    
    private async void OnCapturePhotoClicked(object? sender, EventArgs e)
    {
        try
        {
            FileResult? photo = await MediaPicker.Default.CapturePhotoAsync();
            if (photo is null)
            {
                return;
            }

            await using Stream photoStream = await photo.OpenReadAsync();
            using MemoryStream memoryStream = new ();
            await photoStream.CopyToAsync(memoryStream);
            byte[] imageBytes = memoryStream.ToArray();
            _beverage.PhotoImage = imageBytes;
            CapturedPhotoImage.Source = ImageSource.FromStream(() => new MemoryStream(imageBytes));
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Fel", $"Ett fel uppstod vid fotografering: {ex.Message}", "OK");
        }
    }

    private async Task LoadRemarksAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _dataContext.Database.EnsureCreatedAsync(cancellationToken);
            List<Remark> remarks     = await AsyncEnumerable.ToListAsync(
                _dataContext.Remarks.AsAsyncEnumerable(), cancellationToken);

            RemarkPicker.ItemsSource = remarks;
            if (remarks.Count > 0)
            {
                int? selectedRemarkId = _beverage.ReviewRemark?.Id;
                RemarkPicker.SelectedItem = remarks.FirstOrDefault(
                    remark => remark.Id == selectedRemarkId) ?? remarks[0];
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Fel", $"Ett fel uppstod i laddningen: {ex}", "OK"); // Display an error message if an exception occurs
        }
    }
    private async Task LoadCountriesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _dataContext.Database.EnsureCreatedAsync(cancellationToken);
            List<Country> countries = await AsyncEnumerable.ToListAsync(
                _dataContext.Countries.AsAsyncEnumerable(), cancellationToken);

            CountryPicker.ItemsSource = countries;
            if (countries.Count > 0)
            {
                int? selectedCountryId = _beverage.CountryOfOrigin?.Id;
                CountryPicker.SelectedItem = countries.FirstOrDefault(
                    country => country.Id == selectedCountryId) ?? countries[0];
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Fel", $"Ett fel uppstod i laddningen: {ex}", "OK"); // Display an error message if an exception occurs
        }
    }
    private async Task LoadCategoriesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await _dataContext.Database.EnsureCreatedAsync(cancellationToken);
            List<BeverageCategory> beverageCategories = await AsyncEnumerable.ToListAsync(
                _dataContext.BeverageCategories.AsAsyncEnumerable(), cancellationToken);

            CategoryPicker.ItemsSource = beverageCategories;
            if (beverageCategories.Count > 0)
            {
                int? selectedCategoryId = _beverage.Category?.Id;
                CategoryPicker.SelectedItem = beverageCategories.FirstOrDefault(
                    category => category.Id == selectedCategoryId) ?? beverageCategories[0];
            }
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Fel", $"Ett fel uppstod i laddningen: {ex}", "OK"); // Display an error message if an exception occurs
        }
    }
    
    private async void OnDeleteBeverageClicked(object? sender, EventArgs e)
    {
        if (await _contentPageHelper.ConfirmDeleteAsync(_beverage, "drycken"))
        {
            try
            {
                _dataContext.Beverages.Remove(_beverage!); // Remove the selected drink from the DbSet
                await _dataContext.SaveChangesAsync(); // Save changes to the database
                await Navigation.PopAsync(); // Navigate back to the previous page after deletion
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Fel", $"Ett fel uppstod vid borttagning av drycken: {ex}", "OK"); // Display an error message if an exception occurs
            }
        }
    }
    private async void OnUpdateBeverageClicked(object? sender, EventArgs e)
    {
        if (sender is Button)
        {
            if (string.IsNullOrEmpty(BeverageNameEntry.Text))
            {
                await DisplayAlertAsync(
                    "Fel",
                    "Ange kategorins namn.",
                    "OK"); 
                return;
            }
            try
            {
                int id = 0; ;
                if (!string.IsNullOrEmpty(BeverageIdEntry.Text)) id = int.Parse(BeverageIdEntry.Text.Trim());
                _beverage.Name = BeverageNameEntry.Text.Trim();
                _beverage.Category = CategoryPicker.SelectedItem as BeverageCategory
                    ?? throw new InvalidOperationException("Select a beverage category.");
                _beverage.CountryOfOrigin = CountryPicker.SelectedItem as Country
                    ?? throw new InvalidOperationException("Select a country of origin.");
                _beverage.ReviewRemark = RemarkPicker.SelectedItem as Remark
                    ?? throw new InvalidOperationException("Select a remark.");
                if (id > 0)
                {
                    _beverage.Id = id;
                    _dataContext.Beverages.Update(_beverage);
                }
                else 
                {
                    _dataContext.Beverages.Add(_beverage);
                }
                if (await _dataContext.Beverages.AnyAsync(x => x.Name == BeverageNameEntry.Text.Trim() && x.Id != _beverage.Id))
                {
                    await DisplayAlertAsync("Fel", $"Namnet {BeverageNameEntry.Text.Trim()} är redan registrerat.", "OK");
                    return;
                }
                try {
                    await _dataContext.SaveChangesAsync();
                }
                catch(DbUpdateException dbEx) when (dbEx.InnerException is SqliteException sqliteEx && sqliteEx.SqliteErrorCode ==  19)
                {
                    await DisplayAlertAsync("Fel", $"Gick inte att spara, {dbEx}", "OK");
                    return;
                };
                var beverage = await _dataContext.Beverages.FirstAsync(x => x.Name == BeverageNameEntry.Text.Trim(), _cancellationToken);
                BeverageIdEntry.Text = beverage.Id.ToString();
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
