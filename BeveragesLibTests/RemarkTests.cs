using BeveragesLib.Models;

namespace BeveragesLibTests;

[TestClass]
public sealed class RemarkTests
{
    [TestMethod]
    public void Seed_ReturnsExpectedRemarksWithUnsetIdsAndOrderedValues()
    {
        List<Remark> remarks = Remark.Seed();

        Assert.AreSequenceEqual(
            ["Horrible", "Not Good", "Average", "Good", "Excellent"],
            [.. remarks.Select(remark => remark.Name)]);
        Assert.AreSequenceEqual(
            [1, 2, 3, 4, 5],
            [.. remarks.Select(remark => remark.Order)]);
        Assert.IsTrue(remarks.All(remark => remark.Id == 0));
    }
}
