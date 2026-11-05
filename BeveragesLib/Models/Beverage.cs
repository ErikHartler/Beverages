namespace BeveragesLib.Models;

//User is a Domain Model class that represents a user in the application. It contains properties for the user's Id and Name.
public class Beverage
{
    public int Id { get; set; }  
    public required string Name { get; set; }  
    public required BeverageCategory Category { get; set; }
    public required Remark ReviewRemark { get; set; }
    public required Country CountryOfOrigin { get; set; }
    public required decimal Price { get; set; }
    public DateTime DateOfPurchase { get; set; }

    public string Description { get; set; }
    public byte[]? PhotoImage { get; set; }  // Property to store the image as a byte array

}
