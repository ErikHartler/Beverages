namespace BeveragesLib.Models;

//Country is a Domain Model class that represents a country in the application. It contains properties for the country's Id and Name.
public class Country
{
    public int Id { get; set; }  
    public required string Name { get; set; }

    public static List<Country> Seed()
    {
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
}
