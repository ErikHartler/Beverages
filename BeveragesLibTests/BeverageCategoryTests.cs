using BeveragesLib.Models;

namespace BeveragesLibTests;

[TestClass]
public sealed class BeverageCategoryTests
{
    private static readonly string[] expected = ["Kaffe", "Te", "Vin", "Sprit"];

    [TestMethod]
    public void Seed_ReturnsExpectedCategoriesWithUnsetIds()
    {
        var categories = BeverageCategory.Seed();

        Assert.AreSequenceEqual(
            expected, [.. categories.Select(category => category.Name)]);
        Assert.IsTrue(categories.All(category => category.Id == 0));
    }
}
