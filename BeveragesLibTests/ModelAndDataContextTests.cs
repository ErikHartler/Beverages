using BeveragesLib.Data;
using BeveragesLib.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;



namespace BeveragesLibTests;

[TestClass]
public sealed class DataContextTests
{
    private SqliteConnection _connection = null!;
    private DataContext _dataContext = null!;

    [TestInitialize]
    public async Task InitializeAsync()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        await _connection.OpenAsync(TestContext.CancellationToken);

        var options = new DbContextOptionsBuilder<DataContext>()
            .UseSqlite(_connection)
            .Options;
        _dataContext = new DataContext(options);
        await _dataContext.Database.EnsureCreatedAsync(TestContext.CancellationToken);
    }

    [TestCleanup]
    public async Task CleanupAsync()
    {
        await _dataContext.DisposeAsync();
        await _connection.DisposeAsync();
    }

    [TestMethod]
    public void Model_ConfiguresEntityIdsAsGeneratedOnAdd()
    {
        var categoryId = _dataContext.Model
            .FindEntityType(typeof(BeverageCategory))!
            .FindProperty(nameof(BeverageCategory.Id))!;
        var beverageId = _dataContext.Model
            .FindEntityType(typeof(Beverage))!
            .FindProperty(nameof(Beverage.Id))!;

        Assert.AreEqual(ValueGenerated.OnAdd, categoryId.ValueGenerated);
        Assert.AreEqual(ValueGenerated.OnAdd, beverageId.ValueGenerated);
    }

    [TestMethod]
    public async Task SaveChanges_GeneratesIdsAndPersistsBeverageCategoryAndPhoto()
    {
        var category = new BeverageCategory { Name = "Kaffe" };
        _dataContext.BeverageCategories.Add(category);
        await _dataContext.SaveChangesAsync(TestContext.CancellationToken);

        var beverage = new Beverage
        {
            Name = "Espresso",
            Category = category,
            PhotoImage = [1, 2, 3]
        };
        _dataContext.Beverages.Add(beverage);
        await _dataContext.SaveChangesAsync(TestContext.CancellationToken);

        Assert.IsGreaterThan(0, category.Id);
        Assert.IsGreaterThan(0, beverage.Id);

        _dataContext.ChangeTracker.Clear();
        var persistedBeverage = await _dataContext.Beverages
            .Include(item => item.Category)
            .SingleAsync(TestContext.CancellationToken);

        Assert.AreEqual("Espresso", persistedBeverage.Name);
        Assert.AreEqual("Kaffe", persistedBeverage.Category.Name);
        Assert.IsNotNull(persistedBeverage.PhotoImage);
        Assert.AreSequenceEqual(new byte[] { 1, 2, 3 }, persistedBeverage.PhotoImage!);
    }

    public TestContext TestContext { get; set; }
}
