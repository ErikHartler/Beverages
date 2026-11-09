using BeveragesLib.Data;
using BeveragesLib.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace BeveragesApp;

public partial class RemarkListPage : ContentPage
{
    #region private members
    private readonly DataContext _dataContext;
    private readonly CancellationToken _cancellationToken;
    private readonly ContentPageHelper _contentPageHelper;

    #endregion
    #region constructor
    public RemarkListPage(DataContext dataContext, CancellationToken cancellationToken)
    {
        InitializeComponent();
        _dataContext = dataContext;
        _cancellationToken = cancellationToken;
        _contentPageHelper = new ContentPageHelper(this, _dataContext, _cancellationToken);
        LoadRemarks(CancellationToken.None); 
    }
    #endregion
    #region public  properties
    public ObservableCollection<Remark> Remarks{ get; set; } = [];
    #endregion

    #region private methods
    private async void OnQuitClicked(object? sender, EventArgs e)
    {
        _contentPageHelper.OnQuitClicked(sender, e);
    }
    private async void LoadRemarks(CancellationToken cancellationToken)
    {
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
            await DisplayAlertAsync("Fel", $"Ett fel uppstod i laddningen: {ex}", "OK"); // Display an error message if an exception occurs
        }
    }
    #endregion
    #region private event handlers
    private async void OnAddRemarkClicked(object? sender, EventArgs e)
    {
        try
        {
            RemarkItemPage page = new(_dataContext, new Remark() { Name = "", Order = 0 }, _cancellationToken);
            await Navigation.PushAsync(page);
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Fel", $"Ett fel uppstod: {ex}", "OK"); // Display an error message if an exception occurs
        }
    }
    
    private async void OnDeleteRemarkClicked(object? sender, EventArgs e)
    {
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
                await DisplayAlertAsync("Fel", $"Ett fel uppstod vid borttagning av kategorin: {ex}", "OK"); // Display an error message if an exception occurs
            }
        }
      
    }
    private async void OnUpdateRemarkClicked(object? sender, EventArgs e)
    {
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
                        c => c.Id == selectedRemark.Id, CancellationToken.None
                    ); 
                RemarkItemPage page = new(_dataContext, remark! , _cancellationToken);
                await Navigation.PushAsync(page);
            }
            catch (Exception ex)
            {
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
        await Navigation.PopAsync();
    }

}
