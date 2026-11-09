using BeveragesApp.PlaywrightTestApp;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;

namespace BeveragesApp.PlaywrightTests;

[TestClass]
[DoNotParallelize]
public class GoBackNavigationTests : PageTest
{
    private static WebApplication? _testApp;
    private static string _baseUrl = string.Empty;

    [ClassInitialize]
    public static async Task StartTestApp(TestContext _)
    {
        _testApp = WebTestApp.Create();
        _testApp.Urls.Add("http://127.0.0.1:0");
        await _testApp.StartAsync();
        _baseUrl = _testApp.Services.GetRequiredService<IServer>()
            .Features.Get<IServerAddressesFeature>()!.Addresses.Single();
    }

    [ClassCleanup]
    public static async Task StopTestApp()
    {
        if (_testApp is not null)
        {
            await _testApp.StopAsync();
            await _testApp.DisposeAsync();
        }
    }

    [TestMethod]
    public async Task BeverageListAndItemBackButtonsReturnToPreviousPage()
    {
        await Page.GotoAsync(_baseUrl);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Drycker" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Tillbaka" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Svenska kräftor kräfva dessa drycker" })).ToBeVisibleAsync();

        await Page.GetByRole(AriaRole.Button, new() { Name = "Drycker" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Ändra dryck Testdryck" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Tillbaka" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Drycker" })).ToBeVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Tillbaka" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Svenska kräftor kräfva dessa drycker" })).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task CategoryListAndItemBackButtonsReturnToPreviousPage()
    {
        await Page.GotoAsync(_baseUrl);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Kategorier" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Tillbaka" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Svenska kräftor kräfva dessa drycker" })).ToBeVisibleAsync();

        await Page.GetByRole(AriaRole.Button, new() { Name = "Kategorier" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Ändra kategori Öl" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Tillbaka" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Dryckeskategorier" })).ToBeVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Tillbaka" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Svenska kräftor kräfva dessa drycker" })).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task CountryListAndItemBackButtonsReturnToPreviousPage()
    {
        await Page.GotoAsync(_baseUrl);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Länder" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Tillbaka" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Svenska kräftor kräfva dessa drycker" })).ToBeVisibleAsync();

        await Page.GetByRole(AriaRole.Button, new() { Name = "Länder" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Ändra land Sverige" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Tillbaka" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Länder" })).ToBeVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Tillbaka" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Svenska kräftor kräfva dessa drycker" })).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task RemarkListAndItemBackButtonsReturnToPreviousPage()
    {
        await Page.GotoAsync(_baseUrl);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Graderingar" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Tillbaka" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Svenska kräftor kräfva dessa drycker" })).ToBeVisibleAsync();

        await Page.GetByRole(AriaRole.Button, new() { Name = "Graderingar" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Ändra gradering Bra" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Tillbaka" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Graderingar" })).ToBeVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Tillbaka" }).ClickAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Svenska kräftor kräfva dessa drycker" })).ToBeVisibleAsync();
    }
}
