using BeveragesLib.Data;
using BeveragesLib.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace BeveragesLibTests.DbTests;

[TestClass]
public sealed class RemarkTests
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
    public void Seed_ReturnsExpectedRemarksWithUnsetIdsAndOrderedValues()
    {
        List<Remark> remarks = Remark.Seed();

        Assert.AreSequenceEqual(
            ["Hemskt", "Inte bra", "Medelmåttligt", "Bra", "Strålande"],
            [.. remarks.Select(remark => remark.Name)]);
        Assert.AreSequenceEqual(
            [1, 2, 3, 4, 5],
            [.. remarks.Select(remark => remark.Order)]);
        Assert.IsTrue(remarks.All(remark => remark.Id == 0));
    }

    [TestMethod]
    public async Task Add_RemarkPersistsNameAndOrderWithGeneratedId()
    {
        Remark remark = new() { Name = "Utmärkt", Order = 6 };

        _dataContext.Remarks.Add(remark);
        await _dataContext.SaveChangesAsync(TestContext.CancellationToken);

        Assert.IsGreaterThan(0, remark.Id);
        _dataContext.ChangeTracker.Clear();
        Remark persisted = await _dataContext.Remarks
            .SingleAsync(item => item.Id == remark.Id, TestContext.CancellationToken);
        Assert.AreEqual("Utmärkt", persisted.Name);
        Assert.AreEqual(6, persisted.Order);
    }

    [TestMethod]
    public async Task Update_RemarkPersistsChangedNameAndOrder()
    {
        Remark remark = new() { Name = "Bra", Order = 4 };
        _dataContext.Remarks.Add(remark);
        await _dataContext.SaveChangesAsync(TestContext.CancellationToken);
        int remarkId = remark.Id;

        remark.Name = "Strålande";
        remark.Order = 5;
        await _dataContext.SaveChangesAsync(TestContext.CancellationToken);

        _dataContext.ChangeTracker.Clear();
        Remark updated = await _dataContext.Remarks
            .SingleAsync(item => item.Id == remarkId, TestContext.CancellationToken);
        Assert.AreEqual("Strålande", updated.Name);
        Assert.AreEqual(5, updated.Order);
    }

    [TestMethod]
    public async Task Delete_RemarkRemovesRecord()
    {
        Remark remark = new() { Name = "Tillfälligt", Order = 1 };
        _dataContext.Remarks.Add(remark);
        await _dataContext.SaveChangesAsync(TestContext.CancellationToken);
        int remarkId = remark.Id;

        _dataContext.Remarks.Remove(remark);
        await _dataContext.SaveChangesAsync(TestContext.CancellationToken);

        Assert.IsNull(await _dataContext.Remarks.FindAsync([remarkId], TestContext.CancellationToken));
        Assert.AreEqual(0, await _dataContext.Remarks.CountAsync(TestContext.CancellationToken));
    }

    [TestMethod]
    public async Task Add_DuplicateRemarkNameIsRejected()
    {
        _dataContext.Remarks.Add(new Remark { Name = "Bra", Order = 1 });
        await _dataContext.SaveChangesAsync(TestContext.CancellationToken);
        _dataContext.Remarks.Add(new Remark { Name = "Bra", Order = 2 });

        DbUpdateException exception = await Assert.ThrowsAsync<DbUpdateException>(
            async () => await _dataContext.SaveChangesAsync(TestContext.CancellationToken));

        Assert.IsInstanceOfType<SqliteException>(exception.InnerException);
        SqliteException sqliteException = (SqliteException)exception.InnerException!;
        Assert.AreEqual(19, sqliteException.SqliteErrorCode);
        Assert.AreEqual(1, await _dataContext.Remarks.CountAsync(TestContext.CancellationToken));
    }
}
