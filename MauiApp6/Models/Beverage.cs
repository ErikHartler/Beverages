namespace DrinkManagement.Models;

//User is a Domain Model class that represents a user in the application. It contains properties for the user's Id and Name.
public class Beverage
{
    public int Id { get; set; } // Primary key for the Drink entity
    public required string Name { get; set; } // Name of the drink
}
