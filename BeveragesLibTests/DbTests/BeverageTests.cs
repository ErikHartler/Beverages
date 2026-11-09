using BeveragesLib.Data;
using BeveragesLib.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace BeveragesLibTests.DbTests;

[TestClass]
public sealed class BeverageTests
{
    private SqliteConnection _connection = null!;
    private DataContext _dataContext = null!;
    private BeverageCategory _category = null!;
    private Country _country = null!;
    private Remark _remark = null!;

    public TestContext TestContext { get; set; } = null!;

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

        _category = new BeverageCategory { Name = "Kaffe" };
        _country = new Country { Name = "Sverige" };
        _remark = new Remark { Name = "Bra", Order = 4 };
        _dataContext.AddRange(_category, _country, _remark);
        await _dataContext.SaveChangesAsync(TestContext.CancellationToken);
    }

    [TestCleanup]
    public async Task CleanupAsync()
    {
        await _dataContext.DisposeAsync();
        await _connection.DisposeAsync();
    }

    [TestMethod]
    public async Task Add_BeveragePersistsWithItsRelationships()
    {
        DateTime purchaseDate = new(2026, 10, 15);
        byte[] photo = [1, 2, 3, 4];
        Beverage beverage = CreateBeverage("Espresso", 3.50m, purchaseDate, "Dark roast", photo);

        _dataContext.Beverages.Add(beverage);
        await _dataContext.SaveChangesAsync(TestContext.CancellationToken);
        int beverageId = beverage.Id;

        _dataContext.ChangeTracker.Clear();
        Beverage persisted = await LoadBeverageAsync(beverageId);

        Assert.IsGreaterThan(0, beverageId);
        Assert.AreEqual("Espresso", persisted.Name);
        Assert.AreEqual(3.50m, persisted.Price);
        Assert.AreEqual(purchaseDate, persisted.DateOfPurchase);
        Assert.AreEqual("Dark roast", persisted.Description);
        Assert.AreSequenceEqual(photo, persisted.PhotoImage!);
        Assert.AreEqual("Kaffe", persisted.Category.Name);
        Assert.AreEqual("Sverige", persisted.CountryOfOrigin.Name);
        Assert.AreEqual("Bra", persisted.ReviewRemark.Name);
    }

    [TestMethod]
    public async Task Update_BeveragePersistsChangedValues()
    {
        Beverage beverage = CreateBeverage("Espresso", 3.50m, new DateTime(2026, 10, 15), "Dark roast", [1, 2]);
        _dataContext.Beverages.Add(beverage);
        await _dataContext.SaveChangesAsync(TestContext.CancellationToken);
        int beverageId = beverage.Id;

        beverage.Name = "Flat White";
        beverage.Price = 4.25m;
        beverage.DateOfPurchase = new DateTime(2026, 11, 1);
        beverage.Description = "Oat milk";
        beverage.PhotoImage = [8, 9, 10];
        await _dataContext.SaveChangesAsync(TestContext.CancellationToken);

        _dataContext.ChangeTracker.Clear();
        Beverage updated = await LoadBeverageAsync(beverageId);

        Assert.AreEqual("Flat White", updated.Name);
        Assert.AreEqual(4.25m, updated.Price);
        Assert.AreEqual(new DateTime(2026, 11, 1), updated.DateOfPurchase);
        Assert.AreEqual("Oat milk", updated.Description);
        Assert.AreSequenceEqual(new byte[] { 8, 9, 10 }, updated.PhotoImage!);
        Assert.AreEqual("Kaffe", updated.Category.Name);
        Assert.AreEqual("Sverige", updated.CountryOfOrigin.Name);
        Assert.AreEqual("Bra", updated.ReviewRemark.Name);
    }

    [TestMethod]
    public async Task Delete_BeverageRemovesOnlyTheBeverage()
    {
        Beverage beverage = CreateBeverage("Espresso", 3.50m, new DateTime(2026, 10, 15), "Dark roast", null);
        _dataContext.Beverages.Add(beverage);
        await _dataContext.SaveChangesAsync(TestContext.CancellationToken);
        int beverageId = beverage.Id;

        _dataContext.Beverages.Remove(beverage);
        await _dataContext.SaveChangesAsync(TestContext.CancellationToken);

        Assert.IsNull(await _dataContext.Beverages.FindAsync([beverageId], TestContext.CancellationToken));
        Assert.AreEqual(0, await _dataContext.Beverages.CountAsync(TestContext.CancellationToken));
        Assert.AreEqual(1, await _dataContext.BeverageCategories.CountAsync(TestContext.CancellationToken));
        Assert.AreEqual(1, await _dataContext.Countries.CountAsync(TestContext.CancellationToken));
        Assert.AreEqual(1, await _dataContext.Remarks.CountAsync(TestContext.CancellationToken));
    }

    [TestMethod]
    public async Task Add_DuplicateBeverageNameIsRejected()
    {
        _dataContext.Beverages.Add(CreateBeverage("Duplicate name", 3.50m, new DateTime(2026, 10, 15), "First", null));
        await _dataContext.SaveChangesAsync(TestContext.CancellationToken);
        _dataContext.Beverages.Add(CreateBeverage("Duplicate name", 4.00m, new DateTime(2026, 10, 16), "Duplicate", null));

        DbUpdateException exception = await Assert.ThrowsAsync<DbUpdateException>(
            async () => await _dataContext.SaveChangesAsync(TestContext.CancellationToken));

        Assert.IsInstanceOfType<SqliteException>(exception.InnerException);
        SqliteException sqliteException = (SqliteException)exception.InnerException!;
        Assert.AreEqual(19, sqliteException.SqliteErrorCode);
        Assert.AreEqual(1, await _dataContext.Beverages.CountAsync(TestContext.CancellationToken));
    }

    private Beverage CreateBeverage(string name, decimal price, DateTime dateOfPurchase, string description, byte[]? photo) => new()
    {
        Name = name,
        Category = _category,
        CountryOfOrigin = _country,
        ReviewRemark = _remark,
        Price = price,
        DateOfPurchase = dateOfPurchase,
        Description = description,
        PhotoImage = photo
    };

    private Task<Beverage> LoadBeverageAsync(int id) => _dataContext.Beverages
        .Include(item => item.Category)
        .Include(item => item.CountryOfOrigin)
        .Include(item => item.ReviewRemark)
        .SingleAsync(item => item.Id == id, TestContext.CancellationToken);
}
