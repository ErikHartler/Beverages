using System.ComponentModel.DataAnnotations;

namespace BeveragesLib.Models;

//User is a Domain Model class that represents a user in the application. It contains properties for the user's Id and Name.
public class BeverageCategory
{
    [Key]
    public int Id { get; set; }
    public required string Name { get; set; }

    public static List<BeverageCategory> Seed()
    {
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
}
