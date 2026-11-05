
using BeveragesLib.Data;
using BeveragesLib.Models;

namespace BeveragesApp;

public partial class BeverageItemPage : ContentPage
{
    #region private members
    private readonly DataContext _dataContext;
    private readonly CancellationToken _cancellationToken = new();
    private readonly ContentPageHelper _contentPageHelper;

    private readonly  Beverage _beverage;
    #endregion
    #region constructor
    public BeverageItemPage(DataContext dataContext, Beverage beverage)
    {
        InitializeComponent();
        _dataContext = dataContext;
        _beverage = beverage;
        _contentPageHelper = new ContentPageHelper(this, _dataContext, _cancellationToken);
#if ANDROID
        _contentPageHelper.AddAndroidMenuItem("Drycker", _contentPageHelper.OnShowBeveragesClicked);
        _contentPageHelper.AddAndroidMenuItem("Kategorier", _contentPageHelper.OnShowCategoriesClicked);
        _contentPageHelper.AddAndroidMenuItem("Avsluta", _contentPageHelper.OnQuitClicked);
#endif
        BindingContext = this; 
        BeverageIdEntry.Text = beverage.Id.ToString();
        BeverageNameEntry.Text = beverage.Name;
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
        await LoadCategoriesAsync(CancellationToken.None);
    }
    #endregion
    #region private event handlers
    private async void OnQuitClicked(object? sender, EventArgs e)
    {
        _contentPageHelper.OnQuitClicked(sender, e);
    }
    
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
        if (int.Parse(BeverageIdEntry.Text.Trim()) == 0) return;
        if (sender is Button button && button.CommandParameter is Beverage selectedBeverage)
        {
            bool confirmDelete = await DisplayAlertAsync("Bekräfta borttagning", $"Är du säker på att du vill ta bort drycken '{selectedBeverage.Name}'?", "Yes", "No"); // Ask for confirmation before deleting
            if (!confirmDelete)
            {
                return; 
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
            if (string.IsNullOrEmpty(BeverageNameEntry.Text))
            {
                await DisplayAlertAsync(
                    "Fel",
                    "ange kategorins namn.",
                    "OK"); // Display an error message if the new category name is empty
                return;
            }
            try
            {
                int id = 0; ;
                if (!string.IsNullOrEmpty(BeverageIdEntry.Text)) id = int.Parse(BeverageIdEntry.Text.Trim());
                _beverage.Name = BeverageNameEntry.Text.Trim();
                BeverageCategory selected = (CategoryPicker.SelectedItem as BeverageCategory)!;
                BeverageCategory category = new() { Id = selected.Id, Name = selected.Name };
                _beverage.Category = selected;
                if (id > 0)
                {
                    _beverage.Id = id;
                    _dataContext.Beverages.Update(_beverage);
                }
                else
                {
                    _dataContext.Beverages.Add(_beverage);
                }
                await _dataContext.SaveChangesAsync();
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
