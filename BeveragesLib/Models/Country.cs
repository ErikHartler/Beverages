using Microsoft.EntityFrameworkCore;

namespace BeveragesLib.Models;

[Index(nameof(Name), IsUnique = true)]
public class Country : IdName
{
    public static List<Country> Seed()
    {
        return
            [
                new Country { Name = "Sverige" },
                new Country { Name = "Norge" },
                new Country { Name = "Danmark" },
                new Country { Name = "Finland" },
                new Country { Name = "Italien" },
                new Country { Name = "Tyskland" },
                new Country { Name = "Spanien" },
                new Country { Name = "Sydafrika" }
            ];
    }
}
