using BeveragesLib.Data;


namespace BeveragesApp
{
    public class ContentPageHelper
    {
        private readonly ContentPage _contentPage;
        private readonly DataContext _dataContext;
        private readonly CancellationToken _cancellationToken;

        public ContentPageHelper(ContentPage contentPage,DataContext dataContext, CancellationToken cancellationToken)
        {
            _contentPage = contentPage;
            _dataContext = dataContext;
            _cancellationToken = cancellationToken;
#if ANDROID
            AddAndroidMenuItem("Drycker", OnShowBeveragesClicked);
            AddAndroidMenuItem("Kategorier", OnShowCategoriesClicked);
            AddAndroidMenuItem("Kategorier", OnShowCategoriesClicked);
            AddAndroidMenuItem("Länder", OnShowCountriesClicked);
            AddAndroidMenuItem("Betyg", OnShowRemarksClicked);
            AddAndroidMenuItem("Avsluta", OnQuitClicked);
#endif
        }

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

        public async void OnShowCountriesClicked(object? sender, EventArgs e)
        {
            try
            {
                CountryListPage page = new(_dataContext, _cancellationToken);
                await _contentPage.Navigation.PushAsync(page);
            }
            catch (Exception ex)
            {
                await _contentPage.DisplayAlertAsync("Fel", $"Ett fel uppstod: {ex}", "OK");
            }
        }
        public async void OnShowCategoriesClicked(object? sender, EventArgs e)
        {
            try
            {
                CategoryListPage page = new(_dataContext, _cancellationToken);
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
                BeverageListPage page = new(_dataContext, _cancellationToken);
                await _contentPage.Navigation.PushAsync(page);
            }
            catch (Exception ex)
            {
                await _contentPage.DisplayAlertAsync("Fel", $"Ett fel uppstod: {ex}", "OK");
            }
        }
        public async void OnShowRemarksClicked(object? sender, EventArgs e)
        {
            try
            {
                RemarkListPage page = new(_dataContext, _cancellationToken);
                await _contentPage.Navigation.PushAsync(page);
            }
            catch (Exception ex)
            {
                await _contentPage.DisplayAlertAsync("Fel", $"Ett fel uppstod: {ex}", "OK");
            }
        }
        

    }
}
