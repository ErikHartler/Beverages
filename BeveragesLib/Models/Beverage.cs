using Microsoft.EntityFrameworkCore;

namespace BeveragesLib.Models;

[Index(nameof(Name), IsUnique = true)]
public class Beverage: IdName
{

    public required BeverageCategory Category { get; set; }
    public required Remark ReviewRemark { get; set; }
    public required Country CountryOfOrigin { get; set; }
    public required decimal Price { get; set; }
    public DateTime DateOfPurchase { get; set; }

    public string? Description { get; set; }
    public byte[]? PhotoImage { get; set; }  // Property to store the image as a byte array

}
