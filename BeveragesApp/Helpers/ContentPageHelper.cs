using BeveragesLib.Data;
using BeveragesLib.Models;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;

namespace BeveragesApp.Helpers
{
    public class ContentPageHelper
    {
        #region private members
        private readonly ContentPage _contentPage;
        private readonly DataContext _dataContext;
        private readonly CancellationToken _cancellationToken;
        #endregion

        #region public constructors
        public ContentPageHelper(ContentPage contentPage, DataContext dataContext, CancellationToken cancellationToken)
        {
            _contentPage = contentPage;
            _dataContext = dataContext;
            _cancellationToken = cancellationToken;

#if ANDROID
            AddAndroidMenuItem("Drycker", OnShowBeveragesClicked);
            AddAndroidMenuItem("Kategorier", OnShowCategoriesClicked);
            AddAndroidMenuItem("Länder", OnShowCountriesClicked);
            AddAndroidMenuItem("Graderingar", OnShowRemarksClicked);
            AddAndroidMenuItem("Avsluta", OnQuitClicked);
#else
            AddMenuBarItems();
#endif
            _contentPage.BindingContext = contentPage;
        }
        #endregion

#if ANDROID
        public void AddAndroidMenuItem(string text, EventHandler handler)
        {
            ToolbarItem item = new() { Text = text, Order = ToolbarItemOrder.Secondary };
            item.Clicked += handler;
            _contentPage.ToolbarItems.Add(item);
        }
#else
        private void AddMenuBarItems()
        {
            MenuBarItem fileMenu = new() { Text = "Arkiv" };
            fileMenu.Add(CreateMenuFlyoutItem("Avsluta", OnQuitClicked));

            MenuBarItem informationMenu = new() { Text = "Information" };
            informationMenu.Add(CreateMenuFlyoutItem("Drycker", OnShowBeveragesClicked));
            informationMenu.Add(CreateMenuFlyoutItem("Kategorier", OnShowCategoriesClicked));
            informationMenu.Add(CreateMenuFlyoutItem("Länder", OnShowCountriesClicked));
            informationMenu.Add(CreateMenuFlyoutItem("Graderingar", OnShowRemarksClicked));

            MenuBarItem viewMenu = new() { Text = "Visa" };
            viewMenu.Add(CreateMenuFlyoutItem("Uppdatera"));
            viewMenu.Add(CreateMenuFlyoutItem("Toggle Light/Dark Mode"));

            MenuBarItem helpMenu = new() { Text = "Hjälp" };

            _contentPage.MenuBarItems.Add(fileMenu);
            _contentPage.MenuBarItems.Add(informationMenu);
            _contentPage.MenuBarItems.Add(viewMenu);
            _contentPage.MenuBarItems.Add(helpMenu);
        }

        private static MenuFlyoutItem CreateMenuFlyoutItem(string text, EventHandler? handler = null)
        {
            MenuFlyoutItem item = new() { Text = text };
            if (handler is not null)
            {
                item.Clicked += handler;
            }

            return item;
        }
#endif
        #region public event handlers
        public async void OnQuitClicked(object? sender, EventArgs e)
        {
            await OnQuitClickedAsync(sender, e);
        }

        public async Task OnQuitClickedAsync(object? sender, EventArgs e)
        {
            using var _ = new ButtonExecutionScope(sender);
            if (await _contentPage.DisplayAlertAsync("Avsluta", "Är du säker?", "Ja", "Nej"))
            {
                Application.Current?.Quit();
            }
        }

        public async void OnShowCountriesClicked(object? sender, EventArgs e)
        {
            using var _ = new ButtonExecutionScope(sender);
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
            using var _ = new ButtonExecutionScope(sender);
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
            using var _ = new ButtonExecutionScope(sender);
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
            using var _ = new ButtonExecutionScope(sender);
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

        public async Task<bool> ConfirmDeleteAsync(IdName sender, string selectedItemName)
        {
            bool confirmDelete = false;
            if (sender !=null)
            {
                if (sender.Id == 0)
                {
                    await _contentPage.DisplayAlertAsync("Fel", $"{selectedItemName} {sender.Name} har inte sparats än och kan inte tas bort.", "OK");
                    return false;
                }
                confirmDelete = await _contentPage.DisplayAlertAsync("Bekräfta borttagning", $"Är du säker på att du vill ta bort {selectedItemName} {sender.Name}?", "Yes", "No");
            }
            else
            {
                await _contentPage.DisplayAlertAsync("Fel", $"Vänligen välj en {selectedItemName} att ta bort.", "OK"); 
            }

            return confirmDelete;   
        }
        #endregion

    }
}
