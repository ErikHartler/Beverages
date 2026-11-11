using BeveragesLib.Data;
using Microsoft.Extensions.Logging;

namespace BeveragesApp;

public partial class MainPage : ContentPage
{
    private readonly ILogger<MainPage> _logger = PageLogging.CreateLogger<MainPage>();
    #region private members
    private readonly DataContext _dataContext;
    private readonly ContentPageHelper _contentPageHelper;
    #endregion


    #region constructor
    public MainPage(DataContext dataContext)
    {
        CancellationToken cancellationToken = new();
        InitializeComponent();
        _logger.LogInformation("{PageName} opened", nameof(MainPage));

        _dataContext = dataContext; // Assign the injected DataContext instance to the private field
        _contentPageHelper = new ContentPageHelper(this, _dataContext, cancellationToken);
        _logger.LogInformation("{PageName} ended", nameof(MainPage));

    }
    #endregion

    #region private event handlers  


    private async void OnQuitClicked(object? sender, EventArgs e)
    {
        _logger.LogDebug("Executing {PageAction}", nameof(OnQuitClicked));
        await _contentPageHelper.OnQuitClickedAsync(sender, e);
    }


    private async void OnShowCategoriesClicked(object? sender, EventArgs e)
    {
        _logger.LogDebug("Executing {PageAction}", nameof(OnShowCategoriesClicked));
        _contentPageHelper.OnShowCategoriesClicked(sender, e);  
    }

    private async void OnShowBeveragesClicked(object? sender, EventArgs e)
    {
        _logger.LogDebug("Executing {PageAction}", nameof(OnShowBeveragesClicked));
        _contentPageHelper.OnShowBeveragesClicked(sender, e);
    }

    private async void OnShowCountriesClicked(object? sender, EventArgs e)
    {
        _logger.LogDebug("Executing {PageAction}", nameof(OnShowCountriesClicked));
        _contentPageHelper.OnShowCountriesClicked(sender, e);
    }
    private async void OnShowRemarksClicked(object? sender, EventArgs e)
    {
        _logger.LogDebug("Executing {PageAction}", nameof(OnShowRemarksClicked));
        _contentPageHelper.OnShowRemarksClicked(sender, e);
    }
    #endregion
}

