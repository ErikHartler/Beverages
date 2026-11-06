using BeveragesLib.Models;

namespace BeveragesLibTests.DbTests;

[TestClass]
public sealed class CountryTests
{
    [TestMethod]
    public void Seed_ReturnsExpectedCountriesWithUnsetIds()
    {
        List<Country> countries = Country.Seed();

        Assert.AreSequenceEqual(
            ["Sverige", "Norge", "Danmark", "Finland", "Italien", "Tyskland", "Spanien", "Sydafrika"],
            [.. countries.Select(country => country.Name)]);
        Assert.IsTrue(countries.All(country => country.Id == 0));
    }
}
