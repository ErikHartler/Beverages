namespace BeveragesLib.Models;

//Remark is a Domain Model class that represents a remark in the application. It contains properties for the remark's Id and Name.
public class Remark
{
    public int Id { get; set; }  
    public int Order { get; set; }
    public required string Name { get; set; }

    public static List<Remark> Seed()
    {
        {
            return
                [
                    new Remark { Name = "Horrible", Order = 1 },
                    new Remark { Name = "Not Good", Order = 2 },
                    new Remark { Name = "Average", Order = 3 },
                    new Remark { Name = "Good", Order = 4 },
                    new Remark { Name = "Excellent", Order = 5 }
                ];
        }
    }

}
