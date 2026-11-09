using BeveragesLib.Data;
using BeveragesLib.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace BeveragesLibTests.DbTests;

[TestClass]
public sealed class CountryTests
{
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
    public void Seed_ReturnsExpectedCountriesWithUnsetIds()
    {
        List<Country> countries = Country.Seed();

        Assert.AreSequenceEqual(
            ["Sverige", "Norge", "Danmark", "Finland", "Italien", "Tyskland", "Spanien", "Sydafrika"],
            [.. countries.Select(country => country.Name)]);
        Assert.IsTrue(countries.All(country => country.Id == 0));
    }

    [TestMethod]
    public async Task Add_CountryPersistsWithGeneratedId()
    {
        Country country = new() { Name = "Island" };

        _dataContext.Countries.Add(country);
        await _dataContext.SaveChangesAsync(TestContext.CancellationToken);

        Assert.IsGreaterThan(0, country.Id);
        _dataContext.ChangeTracker.Clear();
        Country persisted = await _dataContext.Countries
            .SingleAsync(item => item.Id == country.Id, TestContext.CancellationToken);
        Assert.AreEqual("Island", persisted.Name);
    }

    [TestMethod]
    public async Task Update_CountryPersistsChangedName()
    {
        Country country = new() { Name = "Island" };
        _dataContext.Countries.Add(country);
        await _dataContext.SaveChangesAsync(TestContext.CancellationToken);
        int countryId = country.Id;

        country.Name = "Grönland";
        await _dataContext.SaveChangesAsync(TestContext.CancellationToken);

        _dataContext.ChangeTracker.Clear();
        Country updated = await _dataContext.Countries
            .SingleAsync(item => item.Id == countryId, TestContext.CancellationToken);
        Assert.AreEqual("Grönland", updated.Name);
    }

    [TestMethod]
    public async Task Delete_CountryRemovesRecord()
    {
        Country country = new() { Name = "Island" };
        _dataContext.Countries.Add(country);
        await _dataContext.SaveChangesAsync(TestContext.CancellationToken);
        int countryId = country.Id;

        _dataContext.Countries.Remove(country);
        await _dataContext.SaveChangesAsync(TestContext.CancellationToken);

        Assert.IsNull(await _dataContext.Countries.FindAsync([countryId], TestContext.CancellationToken));
        Assert.AreEqual(0, await _dataContext.Countries.CountAsync(TestContext.CancellationToken));
    }

    [TestMethod]
    public async Task Add_DuplicateCountryNameIsRejected()
    {
        _dataContext.Countries.Add(new Country { Name = "Sverige" });
        await _dataContext.SaveChangesAsync(TestContext.CancellationToken);
        _dataContext.Countries.Add(new Country { Name = "Sverige" });

        DbUpdateException exception = await Assert.ThrowsAsync<DbUpdateException>(
            async () => await _dataContext.SaveChangesAsync(TestContext.CancellationToken));

        Assert.IsInstanceOfType<SqliteException>(exception.InnerException);
        SqliteException sqliteException = (SqliteException)exception.InnerException!;
        Assert.AreEqual(19, sqliteException.SqliteErrorCode);
        Assert.AreEqual(1, await _dataContext.Countries.CountAsync(TestContext.CancellationToken));
    }
}
