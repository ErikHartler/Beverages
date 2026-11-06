using Microsoft.EntityFrameworkCore;

namespace BeveragesLib.Models;

[Index(nameof(Name), IsUnique = true)]
public class BeverageCategory : IdName
{
    public static List<BeverageCategory> Seed()
    {
        return
            [
                new BeverageCategory { Name = "Kaffe" },
                new BeverageCategory { Name = "Te" },
                new BeverageCategory { Name = "Vin" },
                new BeverageCategory { Name = "Sprit" }
            ];
    }
}
