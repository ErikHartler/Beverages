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
public class BeveragesContentTests : PageTest
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
    public async Task StartsWithExpectedHomeContent()
    {
        await Page.GotoAsync(_baseUrl);

        await Expect(Page.GetByRole(AriaRole.Heading, new()
        {
            Name = "Svenska kräftor kräfva dessa drycker"
        })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Drycker" })).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task OpensBeveragesAndShowsListContent()
    {
        await Page.GotoAsync(_baseUrl);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Drycker" }).ClickAsync();

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Drycker" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Lägg till dryck" })).ToBeVisibleAsync();
        await Expect(Page.GetByText("Testdryck")).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task AddsBeverageToList()
    {
        await Page.GotoAsync(_baseUrl);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Drycker" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Lägg till dryck" }).ClickAsync();
        await Page.GetByLabel("Dryckens namn").FillAsync("Playwright Ale");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Spara" }).ClickAsync();

        await Expect(Page.GetByText("Playwright Ale")).ToBeVisibleAsync();
    }
}
