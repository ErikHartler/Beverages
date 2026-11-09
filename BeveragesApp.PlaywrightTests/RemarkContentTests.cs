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
public class RemarkContentTests : PageTest
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
    public async Task RemarkListShowsExpectedContent()
    {
        await Page.GotoAsync(_baseUrl);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Graderingar" }).ClickAsync();

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Graderingar" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Lägg till gradering" })).ToBeVisibleAsync();
        await Expect(Page.Locator("#remark-list").GetByText("Hemskt", new() { Exact = true })).ToBeVisibleAsync();
        await Expect(Page.Locator("#remark-list").GetByText("Strålande", new() { Exact = true })).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task RemarkItemShowsSelectedRemarkAndActions()
    {
        await Page.GotoAsync(_baseUrl);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Graderingar" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Ändra gradering Bra" }).ClickAsync();

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Gradering" })).ToBeVisibleAsync();
        await Expect(Page.GetByLabel("Gradering ID")).ToHaveValueAsync("4");
        await Expect(Page.GetByLabel("Graderingens ordningsnr")).ToHaveValueAsync("4");
        await Expect(Page.GetByLabel("Graderingens namn")).ToHaveValueAsync("Bra");
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Spara gradering" })).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Ta bort gradering" })).ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task AddsRemarkThroughRemarkItemPage()
    {
        await Page.GotoAsync(_baseUrl);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Graderingar" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Lägg till gradering" }).ClickAsync();

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Gradering" })).ToBeVisibleAsync();
        await Page.GetByLabel("Graderingens ordningsnr").FillAsync("6");
        await Page.GetByLabel("Graderingens namn").FillAsync("Utmärkt");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Spara gradering" }).ClickAsync();

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Graderingar" })).ToBeVisibleAsync();
        await Expect(Page.Locator("#remark-list").GetByText("Utmärkt", new() { Exact = true })).ToBeVisibleAsync();
    }
}
