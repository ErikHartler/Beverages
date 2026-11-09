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
public class CountryContentTests : PageTest
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
    public async Task CountryListShowsExpectedContent()
    {
        await Page.GotoAsync(_baseUrl);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Länder" }).ClickAsync();

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Länder" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Lägg till land" })).ToBeVisibleAsync();
        await Expect(Page.Locator("#country-list").GetByText("Sverige", new() { Exact = true })).ToBeVisibleAsync();
        await Expect(Page.Locator("#country-list").GetByText("Norge", new() { Exact = true })).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task CountryItemShowsSelectedCountryAndActions()
    {
        await Page.GotoAsync(_baseUrl);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Länder" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Ändra land Sverige" }).ClickAsync();

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Land" })).ToBeVisibleAsync();
        await Expect(Page.GetByLabel("Landets ID")).ToHaveValueAsync("1");
        await Expect(Page.GetByLabel("Landets namn")).ToHaveValueAsync("Sverige");
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Spara land" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Ta bort land" })).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task AddsCountryThroughCountryItemPage()
    {
        await Page.GotoAsync(_baseUrl);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Länder" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Lägg till land" }).ClickAsync();

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Land" })).ToBeVisibleAsync();
        await Page.GetByLabel("Landets namn").FillAsync("Island");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Spara land" }).ClickAsync();

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Länder" })).ToBeVisibleAsync();
        await Expect(Page.Locator("#country-list").GetByText("Island", new() { Exact = true })).ToBeVisibleAsync();
    }
}
