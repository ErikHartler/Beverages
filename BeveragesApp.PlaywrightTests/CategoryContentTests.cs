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
public class CategoryContentTests : PageTest
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
    public async Task CategoryListShowsExpectedContent()
    {
        await Page.GotoAsync(_baseUrl);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Kategorier" }).ClickAsync();

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Dryckeskategorier" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Lägg till kategori" })).ToBeVisibleAsync();
        await Expect(Page.Locator("#category-list").GetByText("Öl", new() { Exact = true })).ToBeVisibleAsync();
        await Expect(Page.Locator("#category-list").GetByText("Vin", new() { Exact = true })).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task CategoryItemShowsSelectedCategoryAndActions()
    {
        await Page.GotoAsync(_baseUrl);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Kategorier" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Ändra kategori Öl" }).ClickAsync();

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Dryckeskategori" })).ToBeVisibleAsync();
        await Expect(Page.GetByLabel("Kategorins ID")).ToHaveValueAsync("1");
        await Expect(Page.GetByLabel("Kategorins namn")).ToHaveValueAsync("Öl");
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Spara kategori" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Ta bort kategori" })).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task AddsCategoryThroughCategoryItemPage()
    {
        await Page.GotoAsync(_baseUrl);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Kategorier" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Lägg till kategori" }).ClickAsync();

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Dryckeskategori" })).ToBeVisibleAsync();
        await Page.GetByLabel("Kategorins namn").FillAsync("Alkoholfritt");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Spara kategori" }).ClickAsync();

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Dryckeskategorier" })).ToBeVisibleAsync();
        await Expect(Page.GetByText("Alkoholfritt", new() { Exact = true })).ToBeVisibleAsync();
    }
}
