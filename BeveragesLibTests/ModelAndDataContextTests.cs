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
        var countryId = _dataContext.Model
            .FindEntityType(typeof(Country))!
            .FindProperty(nameof(Country.Id))!;
        var remarkId = _dataContext.Model
            .FindEntityType(typeof(Remark))!
            .FindProperty(nameof(Remark.Id))!;

        Assert.AreEqual(ValueGenerated.OnAdd, categoryId.ValueGenerated);
        Assert.AreEqual(ValueGenerated.OnAdd, beverageId.ValueGenerated);
        Assert.AreEqual(ValueGenerated.OnAdd, countryId.ValueGenerated);
        Assert.AreEqual(ValueGenerated.OnAdd, remarkId.ValueGenerated);
    }

    [TestMethod]
    public async Task SaveChanges_GeneratesIdsAndPersistsBeverageRelationshipsAndPhoto()
    {
        var category = new BeverageCategory { Name = "Kaffe" };
        var country = new Country { Name = "Sverige" };
        var remark = new Remark { Name = "Excellent", Order = 5 };
        _dataContext.AddRange(category, country, remark);
        await _dataContext.SaveChangesAsync(TestContext.CancellationToken);

        var beverage = new Beverage
        {
            Name = "Espresso",
            Category = category,
            CountryOfOrigin = country,
            ReviewRemark = remark,
            Price = 3.50m,
            Description = "Strong coffee",
            PhotoImage = [1, 2, 3]
        };
        _dataContext.Beverages.Add(beverage);
        await _dataContext.SaveChangesAsync(TestContext.CancellationToken);

        Assert.IsGreaterThan(0, category.Id);
        Assert.IsGreaterThan(0, country.Id);
        Assert.IsGreaterThan(0, remark.Id);
        Assert.IsGreaterThan(0, beverage.Id);

        _dataContext.ChangeTracker.Clear();
        var persistedBeverage = await _dataContext.Beverages
            .Include(item => item.Category)
            .Include(item => item.CountryOfOrigin)
            .Include(item => item.ReviewRemark)
            .SingleAsync(TestContext.CancellationToken);

        Assert.AreEqual("Espresso", persistedBeverage.Name);
        Assert.AreEqual("Kaffe", persistedBeverage.Category.Name);
        Assert.AreEqual("Sverige", persistedBeverage.CountryOfOrigin.Name);
        Assert.AreEqual("Excellent", persistedBeverage.ReviewRemark.Name);
        Assert.AreEqual(5, persistedBeverage.ReviewRemark.Order);
        Assert.IsNotNull(persistedBeverage.PhotoImage);
        Assert.AreSequenceEqual(new byte[] { 1, 2, 3 }, persistedBeverage.PhotoImage!);
    }

    public TestContext TestContext { get; set; }
}
