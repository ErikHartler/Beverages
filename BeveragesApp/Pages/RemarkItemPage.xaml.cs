using BeveragesLib.Data;
using BeveragesLib.Models;
using Microsoft.EntityFrameworkCore;

namespace BeveragesApp;

public partial class RemarkItemPage : ContentPage
{
    #region private members
    private readonly DataContext _dataContext;
    private readonly CancellationToken _cancellationToken;
    private readonly ContentPageHelper _contentPageHelper;

    private readonly Remark _remark; // Property to hold the selected beverage category  
    #endregion
    #region constructor
    public RemarkItemPage(DataContext dataContext, Remark remark, CancellationToken cancellationToken)
    {
        InitializeComponent();
        _dataContext = dataContext; // Assign the injected DataContext instance to the private field
        _cancellationToken = cancellationToken;
        _contentPageHelper = new ContentPageHelper(this, _dataContext, _cancellationToken);
        _remark = remark;
          
        RemarkOrderEntry.Text= _remark.Order.ToString();
        RemarkTextEntry.Text = _remark.Name;
        RemarkIdEntry.Text = _remark.Id.ToString();
    }
    #endregion

 
    #region private event handlers
     
    private async void OnDeleteRemarkClicked(object? sender, EventArgs e)
    {
        if (await _contentPageHelper.ConfirmDeleteAsync(_remark, "betyget"))
        {
            try
            {
                _dataContext.Remarks.Remove(_remark); // Remove the selected remark from the DbSet
                await _dataContext.SaveChangesAsync(); // Save changes to the database
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Fel", $"Ett fel uppstod vid borttagning av betyget: {ex}", "OK"); // Display an error message if an exception occurs
            }
        }
        else
        {
            await DisplayAlertAsync("Fel", "Vänligen välj en dryck att ta bort.", "OK"); // Display an error message if no drink is selected
        }
    }
    private async void OnUpdateRemarkClicked(object? sender, EventArgs e)
    {
        if (sender is Button)
        {
            if (string.IsNullOrEmpty(RemarkTextEntry.Text))
            {
                await DisplayAlertAsync(
                    "Fel",
                    "ange betygets namn.",
                    "OK"); // Display an error message if the new remark name is empty
                return;
            }
            try
            {
                int id = int.Parse(RemarkIdEntry.Text.Trim());
                _remark.Name = RemarkTextEntry.Text.Trim();
                if (id >0)
                {
                    _remark.Id = id;
                    _remark.Order = int.Parse(RemarkOrderEntry.Text.Trim());

                    _dataContext.Remarks.Update(_remark);

                }
                else
                {
                    _dataContext.Remarks.Add(_remark);
               }
                if (await _dataContext.Remarks.AnyAsync(x => x.Name == RemarkTextEntry.Text.Trim() && x.Id != _remark.Id))
                {
                    await DisplayAlertAsync("Fel", $"Namnet {RemarkTextEntry.Text.Trim()} är redan registrerat.", "OK");
                    return;
                }

                await _dataContext.SaveChangesAsync();
                var remark = await _dataContext.Remarks.FirstAsync(x => x.Name == RemarkTextEntry.Text.Trim(), _cancellationToken);
                RemarkIdEntry.Text = remark.Id.ToString();
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Fel", $"Ett fel uppstod vid uppdatering av betyget: {ex}", "OK"); // Display an error message if an exception occurs
            }
        }
        else
        {
            await DisplayAlertAsync("Fel", "Välj betyg att uppdatera.", "OK"); // Display an error message if no drink is selected
        }
    }
    #endregion

}
