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
public class ExitButtonTests : PageTest
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
    public async Task MainPageExitButtonAsksForConfirmation()
    {
        await Page.GotoAsync(_baseUrl);
        await AssertExitCanBeCanceledAsync("Svenska kräftor kräfva dessa drycker");
    }

    [TestMethod]
    public async Task BeverageListExitButtonAsksForConfirmation()
    {
        await Page.GotoAsync(_baseUrl);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Drycker" }).ClickAsync();
        await AssertExitCanBeCanceledAsync("Drycker");
    }

    [TestMethod]
    public async Task BeverageItemExitButtonAsksForConfirmation()
    {
        await Page.GotoAsync(_baseUrl);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Drycker" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Ändra dryck Testdryck" }).ClickAsync();
        await AssertExitCanBeCanceledAsync("Dryck");
    }

    [TestMethod]
    public async Task CategoryListExitButtonAsksForConfirmation()
    {
        await Page.GotoAsync(_baseUrl);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Kategorier" }).ClickAsync();
        await AssertExitCanBeCanceledAsync("Dryckeskategorier");
    }

    [TestMethod]
    public async Task CategoryItemExitButtonAsksForConfirmation()
    {
        await Page.GotoAsync(_baseUrl);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Kategorier" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Ändra kategori Öl" }).ClickAsync();
        await AssertExitCanBeCanceledAsync("Dryckeskategori");
    }

    [TestMethod]
    public async Task CountryListExitButtonAsksForConfirmation()
    {
        await Page.GotoAsync(_baseUrl);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Länder" }).ClickAsync();
        await AssertExitCanBeCanceledAsync("Länder");
    }

    [TestMethod]
    public async Task CountryItemExitButtonAsksForConfirmation()
    {
        await Page.GotoAsync(_baseUrl);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Länder" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Ändra land Sverige" }).ClickAsync();
        await AssertExitCanBeCanceledAsync("Land");
    }

    [TestMethod]
    public async Task RemarkListExitButtonAsksForConfirmation()
    {
        await Page.GotoAsync(_baseUrl);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Graderingar" }).ClickAsync();
        await AssertExitCanBeCanceledAsync("Graderingar");
    }

    [TestMethod]
    public async Task RemarkItemExitButtonAsksForConfirmation()
    {
        await Page.GotoAsync(_baseUrl);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Graderingar" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Ändra gradering Bra" }).ClickAsync();
        await AssertExitCanBeCanceledAsync("Gradering");
    }

    private async Task AssertExitCanBeCanceledAsync(string visiblePageHeading)
    {
        await Page.GetByRole(AriaRole.Button, new() { Name = "Avsluta" }).ClickAsync();
        await Expect(Page.Locator("#exit-dialog")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Är du säker?")).ToBeVisibleAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Nej" }).ClickAsync();

        await Expect(Page.Locator("#exit-dialog")).Not.ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = visiblePageHeading })).ToBeVisibleAsync();
    }
}
