
using BeveragesLib.Data;

namespace BeveragesApp;

public partial class MainPage : ContentPage
{
    #region private members
    private readonly DataContext _dataContext;
    private readonly CancellationToken  _cancellationToken  = new();
    public  ContentPageHelper _contentPageHelper { get; set; }
    #endregion


    #region constructor
    public MainPage(DataContext dataContext)
    {
        InitializeComponent();
        _dataContext = dataContext; // Assign the injected DataContext instance to the private field
        BindingContext = this; //MVVM pattern: The BindingContext is set to the MainPage instance itself, allowing data binding between the UI and the code-behind.
        _contentPageHelper = new ContentPageHelper(this, _dataContext, _cancellationToken);
#if ANDROID
        _contentPageHelper.AddAndroidMenuItem("Drycker", _contentPageHelper.OnShowBeveragesClicked);
        _contentPageHelper.AddAndroidMenuItem("Kategorier", _contentPageHelper.OnShowCategoriesClicked);
        _contentPageHelper.AddAndroidMenuItem("Avsluta", _contentPageHelper.OnQuitClicked);
#endif
    }
    #endregion

    #region private event handlers  


    private async void OnQuitClicked(object? sender, EventArgs e)
    {
        _contentPageHelper.OnQuitClicked(sender,e);
    }


    private async void OnShowCategoriesClicked(object? sender, EventArgs e)
    {
        try
        {
            CategoryListPage page = new(_dataContext);
            await Navigation.PushAsync(page);
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Fel", $"Ett fel uppstod: {ex}", "OK");
        }
    }

    private async void OnShowBeveragesClicked(object? sender, EventArgs e)
    {
        try
        {
            BeverageListPage page = new(_dataContext);
            await Navigation.PushAsync(page);
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Fel", $"Ett fel uppstod: {ex}", "OK");
        }
    }
    #endregion
}
