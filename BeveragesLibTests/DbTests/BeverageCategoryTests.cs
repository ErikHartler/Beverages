using BeveragesLib.Data;
using BeveragesLib.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace BeveragesLibTests.DbTests;

[TestClass]
public sealed class BeverageCategoryTests
{
    private static readonly string[] expected = ["Kaffe", "Te", "Vin", "Sprit"];
    private SqliteConnection _connection = null!;
    private DataContext _dataContext = null!;

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
    }

    [TestCleanup]
    public async Task CleanupAsync()
    {
        await _dataContext.DisposeAsync();
        await _connection.DisposeAsync();
    }

    [TestMethod]
    public void Seed_ReturnsExpectedCategoriesWithUnsetIds()
    {
        List<BeverageCategory> categories = BeverageCategory.Seed();

        Assert.AreSequenceEqual(
            expected, [.. categories.Select(category => category.Name)]);
        Assert.IsTrue(categories.All(category => category.Id == 0));
    }

    [TestMethod]
    public async Task Add_CategoryPersistsWithGeneratedId()
    {
        BeverageCategory category = new() { Name = "Alkoholfritt" };

        _dataContext.BeverageCategories.Add(category);
        await _dataContext.SaveChangesAsync(TestContext.CancellationToken);

        Assert.IsGreaterThan(0, category.Id);
        _dataContext.ChangeTracker.Clear();
        BeverageCategory persisted = await _dataContext.BeverageCategories
            .SingleAsync(item => item.Id == category.Id, TestContext.CancellationToken);
        Assert.AreEqual("Alkoholfritt", persisted.Name);
    }

    [TestMethod]
    public async Task Update_CategoryPersistsChangedName()
    {
        BeverageCategory category = new() { Name = "Alkoholfritt" };
        _dataContext.BeverageCategories.Add(category);
        await _dataContext.SaveChangesAsync(TestContext.CancellationToken);
        int categoryId = category.Id;

        category.Name = "Mocktail";
        await _dataContext.SaveChangesAsync(TestContext.CancellationToken);

        _dataContext.ChangeTracker.Clear();
        BeverageCategory updated = await _dataContext.BeverageCategories
            .SingleAsync(item => item.Id == categoryId, TestContext.CancellationToken);
        Assert.AreEqual("Mocktail", updated.Name);
    }

    [TestMethod]
    public async Task Delete_CategoryRemovesRecord()
    {
        BeverageCategory category = new() { Name = "Tillfällig" };
        _dataContext.BeverageCategories.Add(category);
        await _dataContext.SaveChangesAsync(TestContext.CancellationToken);
        int categoryId = category.Id;

        _dataContext.BeverageCategories.Remove(category);
        await _dataContext.SaveChangesAsync(TestContext.CancellationToken);

        Assert.IsNull(await _dataContext.BeverageCategories.FindAsync([categoryId], TestContext.CancellationToken));
        Assert.AreEqual(0, await _dataContext.BeverageCategories.CountAsync(TestContext.CancellationToken));
    }

    [TestMethod]
    public async Task Add_DuplicateCategoryNameIsRejected()
    {
        _dataContext.BeverageCategories.Add(new BeverageCategory { Name = "Kaffe" });
        await _dataContext.SaveChangesAsync(TestContext.CancellationToken);
        _dataContext.BeverageCategories.Add(new BeverageCategory { Name = "Kaffe" });

        DbUpdateException exception = await Assert.ThrowsAsync<DbUpdateException>(
            async () => await _dataContext.SaveChangesAsync(TestContext.CancellationToken));

        Assert.IsInstanceOfType<SqliteException>(exception.InnerException);
        SqliteException sqliteException = (SqliteException)exception.InnerException!;
        Assert.AreEqual(19, sqliteException.SqliteErrorCode);
        Assert.AreEqual(1, await _dataContext.BeverageCategories.CountAsync(TestContext.CancellationToken));
    }
}
