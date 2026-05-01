using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Services;
using StarterApp.Repositories;
using System.Collections.ObjectModel;

namespace StarterApp.ViewModels;

// ViewModel responsible for displaying incoming and outgoing rental requests
public partial class RentalListViewModel : BaseViewModel
{
    // Repository used to get rental request data
    private readonly IItemRepository _repository;

    // Stores the rental requests shown on the page
    [ObservableProperty]
    private ObservableCollection<RentalDto> rentals = new();

    // Default constructor
    public RentalListViewModel()
    {
        Title = "Rental Requests";
    }

    // Main constructor with dependency injection
    public RentalListViewModel(IItemRepository repository)
    {
        _repository = repository;
        Title = "Rental Requests";
    }

    // Command triggered when the user presses the "Incoming" button
    [RelayCommand]
    private async Task LoadIncomingRentalsAsync()
    {
        // Prevent multiple loads at the same time
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            ClearError();

            // Get rental requests made by other users for this user's items
            var result = await _repository.GetIncomingRentalsAsync();

            // Clear old results before showing the new list
            Rentals.Clear();

            foreach (var rental in result)
            {
                Rentals.Add(rental);
            }
        }
        catch (Exception ex)
        {
            SetError($"Failed to load incoming rentals: {ex.Message}");
        }
        finally
        {
            // Reset loading state
            IsBusy = false;
        }
    }

    // Command triggered when the user presses the "Outgoing" button
    [RelayCommand]
    private async Task LoadOutgoingRentalsAsync()
    {
        // Prevent multiple loads at the same time
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            ClearError();

            // Get rental requests made by the logged-in user
            var result = await _repository.GetOutgoingRentalsAsync();

            // Clear old results before showing the new list
            Rentals.Clear();

            foreach (var rental in result)
            {
                Rentals.Add(rental);
            }
        }
        catch (Exception ex)
        {
            SetError($"Failed to load outgoing rentals: {ex.Message}");
        }
        finally
        {
            // Reset loading state
            IsBusy = false;
        }
    }
}