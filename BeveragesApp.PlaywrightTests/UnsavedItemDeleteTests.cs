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
public class UnsavedItemDeleteTests : PageTest
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
    public async Task UnsavedBeverageCannotBeDeletedOrAddedToTheList()
    {
        await Page.GotoAsync(_baseUrl);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Drycker" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Lägg till dryck" }).ClickAsync();
        await Page.GetByLabel("Dryckens namn").FillAsync("Osparad dryck");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Ta bort dryck" }).ClickAsync();

        await AssertUnsavedDeleteErrorAsync("Dryck", "Dryckens namn", "Osparad dryck", "Drycken har inte sparats och kan därför inte tas bort.");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Tillbaka" }).ClickAsync();

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Drycker" })).ToBeVisibleAsync();
        await Expect(Page.Locator("#beverage-list li")).ToHaveCountAsync(1);
        await Expect(Page.Locator("#beverage-list").GetByText("Testdryck", new() { Exact = true })).ToBeVisibleAsync();
        await Expect(Page.GetByText("Osparad dryck", new() { Exact = true })).Not.ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task UnsavedCategoryCannotBeDeletedOrAddedToTheList()
    {
        await Page.GotoAsync(_baseUrl);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Kategorier" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Lägg till kategori" }).ClickAsync();
        await Page.GetByLabel("Kategorins namn").FillAsync("Osparad kategori");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Ta bort kategori" }).ClickAsync();

        await AssertUnsavedDeleteErrorAsync("Dryckeskategori", "Kategorins namn", "Osparad kategori", "Kategorin har inte sparats och kan därför inte tas bort.");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Tillbaka" }).ClickAsync();

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Dryckeskategorier" })).ToBeVisibleAsync();
        await Expect(Page.Locator("#category-list li")).ToHaveCountAsync(2);
        await Expect(Page.Locator("#category-list").GetByText("Öl", new() { Exact = true })).ToBeVisibleAsync();
        await Expect(Page.Locator("#category-list").GetByText("Vin", new() { Exact = true })).ToBeVisibleAsync();
        await Expect(Page.GetByText("Osparad kategori", new() { Exact = true })).Not.ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task UnsavedCountryCannotBeDeletedOrAddedToTheList()
    {
        await Page.GotoAsync(_baseUrl);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Länder" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Lägg till land" }).ClickAsync();
        await Page.GetByLabel("Landets namn").FillAsync("Osparat land");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Ta bort land" }).ClickAsync();

        await AssertUnsavedDeleteErrorAsync("Land", "Landets namn", "Osparat land", "Landet har inte sparats och kan därför inte tas bort.");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Tillbaka" }).ClickAsync();

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Länder" })).ToBeVisibleAsync();
        await Expect(Page.Locator("#country-list li")).ToHaveCountAsync(8);
        await Expect(Page.Locator("#country-list").GetByText("Sverige", new() { Exact = true })).ToBeVisibleAsync();
        await Expect(Page.Locator("#country-list").GetByText("Sydafrika", new() { Exact = true })).ToBeVisibleAsync();
        await Expect(Page.GetByText("Osparat land", new() { Exact = true })).Not.ToBeVisibleAsync();
    }

    [TestMethod]
    public async Task UnsavedRemarkCannotBeDeletedOrAddedToTheList()
    {
        await Page.GotoAsync(_baseUrl);
        await Page.GetByRole(AriaRole.Button, new() { Name = "Graderingar" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Lägg till gradering" }).ClickAsync();
        await Page.GetByLabel("Graderingens namn").FillAsync("Osparad gradering");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Ta bort gradering" }).ClickAsync();

        await AssertUnsavedDeleteErrorAsync("Gradering", "Graderingens namn", "Osparad gradering", "Graderingen har inte sparats och kan därför inte tas bort.");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Tillbaka" }).ClickAsync();

        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Graderingar" })).ToBeVisibleAsync();
        await Expect(Page.Locator("#remark-list li")).ToHaveCountAsync(5);
        await Expect(Page.Locator("#remark-list").GetByText("Hemskt", new() { Exact = true })).ToBeVisibleAsync();
        await Expect(Page.Locator("#remark-list").GetByText("Strålande", new() { Exact = true })).ToBeVisibleAsync();
        await Expect(Page.GetByText("Osparad gradering", new() { Exact = true })).Not.ToBeVisibleAsync();
    }

    private async Task AssertUnsavedDeleteErrorAsync(
        string itemHeading,
        string nameField,
        string enteredName,
        string expectedError)
    {
        await Expect(Page.Locator("#error-dialog")).ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = "Fel" })).ToBeVisibleAsync();
        await Expect(Page.Locator("#error-message")).ToHaveTextAsync(expectedError);
        await Page.GetByRole(AriaRole.Button, new() { Name = "OK" }).ClickAsync();

        await Expect(Page.Locator("#error-dialog")).Not.ToBeVisibleAsync();
        await Expect(Page.GetByRole(AriaRole.Heading, new() { Name = itemHeading })).ToBeVisibleAsync();
        await Expect(Page.GetByLabel(nameField)).ToHaveValueAsync(enteredName);
    }
}
