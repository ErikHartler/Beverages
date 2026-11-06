using BeveragesLib.Data;
using BeveragesLib.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;



namespace BeveragesLibTests.DbTests;

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

        DbContextOptions<DataContext> options = new DbContextOptionsBuilder<DataContext>()
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
        IProperty categoryId = _dataContext.Model
            .FindEntityType(typeof(BeverageCategory))!
            .FindProperty(nameof(BeverageCategory.Id))!;
        IProperty beverageId = _dataContext.Model
            .FindEntityType(typeof(Beverage))!
            .FindProperty(nameof(Beverage.Id))!;
        IProperty countryId = _dataContext.Model
            .FindEntityType(typeof(Country))!
            .FindProperty(nameof(Country.Id))!;
        IProperty remarkId = _dataContext.Model
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
        string beverageName = "Espresso";
        string categoryName = "Kaffe";
        string countryName = "Sverige";
        string remarkName = "Strålande";
        var category = new BeverageCategory { Name = categoryName };
        var country = new Country { Name = countryName };
        var remark = new Remark { Name = remarkName, Order = 5 };
        _dataContext.AddRange(category, country, remark);
        await _dataContext.SaveChangesAsync(TestContext.CancellationToken);

        var beverage = new Beverage
        {
            Name = beverageName,
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
        Beverage persistedBeverage = await _dataContext.Beverages
            .Include(item => item.Category)
            .Include(item => item.CountryOfOrigin)
            .Include(item => item.ReviewRemark)
            .SingleAsync(TestContext.CancellationToken);

        Assert.AreEqual(beverageName, persistedBeverage.Name);
        Assert.AreEqual(categoryName, persistedBeverage.Category.Name);
        Assert.AreEqual(countryName, persistedBeverage.CountryOfOrigin.Name);
        Assert.AreEqual(remarkName, persistedBeverage.ReviewRemark.Name);
        Assert.AreEqual(5, persistedBeverage.ReviewRemark.Order);
        Assert.IsNotNull(persistedBeverage.PhotoImage);
        Assert.AreSequenceEqual(new byte[] { 1, 2, 3 }, persistedBeverage.PhotoImage!);
    }

    public TestContext TestContext { get; set; }
}
