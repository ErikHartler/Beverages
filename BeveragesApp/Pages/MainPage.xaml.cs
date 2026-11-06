using BeveragesLib.Data;

namespace BeveragesApp;

public partial class MainPage : ContentPage
{
    #region private members
    private readonly DataContext _dataContext;
    private readonly CancellationToken  _cancellationToken  = new();
    private readonly ContentPageHelper _contentPageHelper;
    #endregion


    #region constructor
    public MainPage(DataContext dataContext)
    {
        InitializeComponent();
        _dataContext = dataContext; // Assign the injected DataContext instance to the private field
        _contentPageHelper = new ContentPageHelper(this, _dataContext, _cancellationToken);
 
    }
    #endregion

    #region private event handlers  


    private async void OnQuitClicked(object? sender, EventArgs e)
    {
        _contentPageHelper.OnQuitClicked(sender,e);
    }


    private async void OnShowCategoriesClicked(object? sender, EventArgs e)
    {
        _contentPageHelper.OnShowCategoriesClicked(sender, e);  
    }

    private async void OnShowBeveragesClicked(object? sender, EventArgs e)
    {
        _contentPageHelper.OnShowBeveragesClicked(sender, e);
    }

    private async void OnShowCountriesClicked(object? sender, EventArgs e)
    {
        _contentPageHelper.OnShowCountriesClicked(sender, e);
    }
    private async void OnShowRemarksClicked(object? sender, EventArgs e)
    {
        _contentPageHelper.OnShowRemarksClicked(sender, e);
    }
    #endregion
}

