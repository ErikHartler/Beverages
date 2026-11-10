using BeveragesLib.Data;
using BeveragesLib.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

using Microsoft.Extensions.Logging;

namespace BeveragesApp;

public partial class RemarkListPage : ContentPage
{
    private readonly ILogger<RemarkListPage> _logger = PageLogging.CreateLogger<RemarkListPage>();
    #region private members
    private readonly DataContext _dataContext;
    private readonly CancellationToken _cancellationToken;
    private readonly ContentPageHelper _contentPageHelper;

    #endregion
    #region constructor
    public RemarkListPage(DataContext dataContext, CancellationToken cancellationToken)
    {
        InitializeComponent();
        _logger.LogInformation("{PageName} opened", nameof(RemarkListPage));
        _dataContext = dataContext;
        _cancellationToken = cancellationToken;
        _contentPageHelper = new ContentPageHelper(this, _dataContext, _cancellationToken);
        LoadRemarks(_cancellationToken);
        _logger.LogInformation("{PageName} ended", nameof(RemarkListPage));
    }
    #endregion
    #region public  properties
    public ObservableCollection<Remark> Remarks{ get; set; } = [];
    #endregion

    #region private methods
    private async void OnQuitClicked(object? sender, EventArgs e)
    {
        _logger.LogDebug("Executing {PageAction}", nameof(OnQuitClicked));
        await _contentPageHelper.OnQuitClickedAsync(sender, e);
    }
    private async void LoadRemarks(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Executing {PageAction}", nameof(LoadRemarks));
        try
        {
            Remarks.Clear();  

            await _dataContext.Database.EnsureCreatedAsync(cancellationToken); // Ensure the database is created before querying


            List<Remark> remarks = await AsyncEnumerable.ToListAsync(_dataContext.Remarks.AsAsyncEnumerable(), cancellationToken); // Asynchronously retrieve the list of remarks from the database
            foreach (Remark? remark in remarks)
            {
                Remarks.Add(remark); 
            }

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Operation failed on {PageName}", nameof(RemarkListPage));
            await DisplayAlertAsync("Fel", $"Ett fel uppstod i laddningen: {ex}", "OK"); // Display an error message if an exception occurs
        }
    }
    #endregion
    #region private event handlers
    private async void OnAddRemarkClicked(object? sender, EventArgs e)
    {
        _logger.LogDebug("Executing {PageAction}", nameof(OnAddRemarkClicked));
        using var _ = new ButtonExecutionScope(sender);
        try
        {
            RemarkItemPage page = new(_dataContext, new Remark() { Name = "", Order = 0 }, _cancellationToken);
            await Navigation.PushAsync(page);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Operation failed on {PageName}", nameof(RemarkListPage));
            await DisplayAlertAsync("Fel", $"Ett fel uppstod: {ex}", "OK"); // Display an error message if an exception occurs
        }
    }
    
    private async void OnDeleteRemarkClicked(object? sender, EventArgs e)
    {
        _logger.LogDebug("Executing {PageAction}", nameof(OnDeleteRemarkClicked));
        using var _ = new ButtonExecutionScope(sender);
        var selectedRemark = (sender as ImageButton)?.CommandParameter as Remark;
        if (await _contentPageHelper.ConfirmDeleteAsync(selectedRemark!, "betyget"))
        {
            try
            {
                _dataContext.Remarks.Remove(selectedRemark!); // Remove the selected drink from the DbSet
                await _dataContext.SaveChangesAsync(); // Save changes to the database
                Remarks.Remove(selectedRemark!); // Remove the selected drink from the ObservableCollection to update the UI
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Operation failed on {PageName}", nameof(RemarkListPage));
                await DisplayAlertAsync("Fel", $"Ett fel uppstod vid borttagning av kategorin: {ex}", "OK"); // Display an error message if an exception occurs
            }
        }
      
    }
    private async void OnUpdateRemarkClicked(object? sender, EventArgs e)
    {
        _logger.LogDebug("Executing {PageAction}", nameof(OnUpdateRemarkClicked));
        using var _ = new ButtonExecutionScope(sender);
        if (sender is ImageButton button && button.CommandParameter is Remark selectedRemark)
        {
            if (string.IsNullOrEmpty(selectedRemark.Name))
            {
                await DisplayAlertAsync(
                    "Fel",
                    "Vänligen ange ett nytt betygsnamn.",
                    "OK"); // Display an error message if the new category name is empty
                return;
            }
            try
            {
                Remark? remark = await _dataContext.Remarks.FirstOrDefaultAsync(
                        c => c.Id == selectedRemark.Id, _cancellationToken
                    ); 
                RemarkItemPage page = new(_dataContext, remark! , _cancellationToken);
                await Navigation.PushAsync(page);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Operation failed on {PageName}", nameof(RemarkListPage));
                await DisplayAlertAsync("Fel", $"Ett fel uppstod vid uppdatering av betyget: {ex}", "OK"); // Display an error message if an exception occurs
            }
        }
        else
        {
            await DisplayAlertAsync("Fel", "Välj kategori att uppdatera.", "OK"); // Display an error message if no category is selected
        }
    }
    #endregion
    private async void GoBack(object? sender, EventArgs e)
    {
        _logger.LogDebug("Executing {PageAction}", nameof(GoBack));
        using var _ = new ButtonExecutionScope(sender);
        await Navigation.PopAsync();
    }

}
