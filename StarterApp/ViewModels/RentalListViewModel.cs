using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Services;
using StarterApp.Repositories;
using System.Collections.ObjectModel;

namespace StarterApp.ViewModels;

public partial class RentalListViewModel : BaseViewModel
{
    private readonly IItemRepository _repository;

    // Stores the rental requests shown on the page
    [ObservableProperty]
    private ObservableCollection<RentalDto> rentals = new();

    public RentalListViewModel()
    {
        Title = "Rental Requests";
    }

    public RentalListViewModel(IItemRepository repository)
    {
        _repository = repository;
        Title = "Rental Requests";
    }

    [RelayCommand]
    private async Task LoadIncomingRentalsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            ClearError();

            var result = await _repository.GetIncomingRentalsAsync();

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
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task LoadOutgoingRentalsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            ClearError();

            var result = await _repository.GetOutgoingRentalsAsync();

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
            IsBusy = false;
        }
    }
}