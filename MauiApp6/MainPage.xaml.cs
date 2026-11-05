using DrinkManagement.Data;
using DrinkManagement.Models;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;

namespace DrinkManagement; 

public partial class MainPage : ContentPage
{
    private readonly DataContext _dataContext; // Private field to hold the injected DataContext instance
    public ObservableCollection<Beverage> Drinks { get; set; }

    public MainPage(DataContext dataContext)
    {
        InitializeComponent();
        _dataContext = dataContext; // Assign the injected DataContext instance to the private field
        Drinks = [];
        BindingContext = this; //MVVM pattern: The BindingContext is set to the MainPage instance itself, allowing data binding between the UI and the code-behind.

        LoadDrinks(CancellationToken.None); // Load the drinks from the database when the page is initialized
    }

    private async void LoadDrinks(CancellationToken cancellationToken)
    {
        try
        {
            Drinks.Clear(); // Clear the existing drinks in the ObservableCollection

            await _dataContext.Database.EnsureCreatedAsync(cancellationToken); // Ensure the database is created before querying


            var drinks = await AsyncEnumerable.ToListAsync<Beverage>(_dataContext.Drinks.AsAsyncEnumerable(), cancellationToken); // Asynchronously retrieve the list of drinks from the database
            foreach (var drink in drinks)
            {
                Drinks.Add(drink); // Add each retrieved drink to the ObservableCollection
            }

        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", $"An error occurred while loading drinks: {ex}", "OK"); // Display an error message if an exception occurs
        }
    }

    private async void OnAddDrinkClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrEmpty(DrinkNameEntry.Text))
        {
            await DisplayAlertAsync("Error", "Please enter a drink name.", "OK"); // Display an error message if the drink name is empty
            return;
        }

        try
        {
            var newDrink = new Beverage { Name = DrinkNameEntry.Text.Trim() }; // Create a new Drink object with the entered name
            _dataContext.Drinks.Add(newDrink); // Add the new drink to the DbSet
            await _dataContext.SaveChangesAsync(); // Save changes to the database
            Drinks.Add(newDrink); // Add the new drink to the ObservableCollection to update the UI
            DrinkNameEntry.Text = string.Empty; // Clear the entry field after adding the drink
            await DisplayAlertAsync("Success", "Drink added successfully!", "OK"); // Display a success message

        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Error", $"An error occurred while adding the drink: {ex}", "OK"); // Display an error message if an exception occurs
        }
    }
    private async void OnDeleteDrinkClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Beverage selectedDrink)
        {
            bool confirmDelete = await DisplayAlertAsync("Confirm Delete", $"Are you sure you want to delete the drink '{selectedDrink.Name}'?", "Yes", "No"); // Ask for confirmation before deleting
            if (!confirmDelete)
            {
                return; // Exit the method if the user cancels the deletion
            }

            try
            {
                _dataContext.Drinks.Remove(selectedDrink); // Remove the selected drink from the DbSet
                await _dataContext.SaveChangesAsync(); // Save changes to the database
                Drinks.Remove(selectedDrink); // Remove the selected drink from the ObservableCollection to update the UI
                await DisplayAlertAsync("Success", "Drink deleted successfully!", "OK"); // Display a success message
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Error", $"An error occurred while deleting the drink: {ex}", "OK"); // Display an error message if an exception occurs
            }
        }
        else
        {
            await DisplayAlertAsync("Error", "Please select a drink to delete.", "OK"); // Display an error message if no drink is selected
        }
    }
    private async void OnUpdateDrinkClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is Beverage selectedDrink)
        {
            if (string.IsNullOrEmpty(DrinkNameEntry.Text))
            {
                await DisplayAlertAsync("Error", "Please enter a new drink name.", "OK"); // Display an error message if the new drink name is empty
                return;
            }
            try
            {
                selectedDrink.Name = DrinkNameEntry.Text.Trim(); // Update the name of the selected drink
                _dataContext.Drinks.Update(selectedDrink); // Update the selected drink in the DbSet
                await _dataContext.SaveChangesAsync(); // Save changes to the database

                int index = Drinks.IndexOf(selectedDrink); // Find the index of the selected drink in the ObservableCollection
                if (index >= 0)
                {
                    Drinks[index] = selectedDrink; // Update the ObservableCollection to reflect the changes in the UI
                }
                DrinkNameEntry.Text = string.Empty; // Clear the entry field after updating the drink
                await DisplayAlertAsync("Success", "Drink updated successfully!", "OK"); // Display a success message
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Error", $"An error occurred while updating the drink: {ex}", "OK"); // Display an error message if an exception occurs
            }
        }
        else
        {
            await DisplayAlertAsync("Error", "Please select a drink to update.", "OK"); // Display an error message if no drink is selected
        }
    }
}

