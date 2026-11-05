using BeveragesLib.Data;
using BeveragesLib.Models;

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
        BindingContext = this; //MVVM pattern: The BindingContext is set to the BeverageListPage instance itself, allowing data binding between the UI and the code-behind.
        RemarkOrderEntry.Text=remark.Order.ToString();
        RemarkTextEntry.Text = _remark.Name;
        RemarkIdEntry.Text = _remark.Id.ToString();
    }
    #endregion
    #region public properties

    #endregion

    #region private methods
    #endregion
    #region private event handlers

    private async void OnQuitClicked(object? sender, EventArgs e)
    {
        _contentPageHelper.OnQuitClicked(sender, e);
    }
    private async void OnDeleteRemarkClicked(object? sender, EventArgs e)
    {
        if (int.Parse(RemarkIdEntry.Text.Trim()) == 0) return;
        if (sender is Button button && button.CommandParameter is Remark selectedRemark )
        {
            bool confirmDelete = await DisplayAlertAsync("Bekräfta borttagning", $"Är du säker på att du vill ta bort påminnelsen '{selectedRemark.Name}'?", "Yes", "No"); // Ask for confirmation before deleting
            if (!confirmDelete)
            {
                return; // Exit the method if the user cancels the deletion
            }

            try
            {
                _dataContext.Remarks.Remove(selectedRemark); // Remove the selected remark from the DbSet
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
                await _dataContext.SaveChangesAsync(); 
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
