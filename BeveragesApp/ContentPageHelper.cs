
using BeveragesLib.Data;
 

namespace BeveragesApp
{
    public class ContentPageHelper(ContentPage contentPage,DataContext dataContext, CancellationToken cancellationToken)
    {
        private readonly ContentPage _contentPage = contentPage;
        private readonly DataContext _dataContext=dataContext;
        private readonly CancellationToken _cancellationToken = cancellationToken;

#if ANDROID
        public void AddAndroidMenuItem(string text, EventHandler handler)
        {
            ToolbarItem item = new() { Text = text, Order = ToolbarItemOrder.Secondary };
            item.Clicked += handler;
            _contentPage.ToolbarItems.Add(item);
        }
#endif
        public async void OnQuitClicked(object? sender, EventArgs e)
        {
            if (await _contentPage.DisplayAlertAsync("Avsluta", "Är du säker?", "Ja", "Nej"))
            {
                Application.Current?.Quit();
            }
        }


        public async void OnShowCategoriesClicked(object? sender, EventArgs e)
        {
            try
            {
                CategoryListPage page = new(_dataContext);
                await _contentPage.Navigation.PushAsync(page);
            }
            catch (Exception ex)
            {
                await _contentPage.DisplayAlertAsync("Fel", $"Ett fel uppstod: {ex}", "OK");
            }
        }

        public async void OnShowBeveragesClicked(object? sender, EventArgs e)
        {
            try
            {
                BeverageListPage page = new(_dataContext);
                await _contentPage.Navigation.PushAsync(page);
            }
            catch (Exception ex)
            {
                await _contentPage.DisplayAlertAsync("Fel", $"Ett fel uppstod: {ex}", "OK");
            }
        }

    }
}
