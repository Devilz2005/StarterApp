using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Services;
using System.Collections.ObjectModel;

namespace StarterApp.ViewModels;

public partial class RentalListViewModel : BaseViewModel
{
    private readonly IItemService _itemService;

    // Stores the rental requests shown on the page
    [ObservableProperty]
    private ObservableCollection<RentalDto> rentals = new();

    public RentalListViewModel()
    {
        Title = "Rental Requests";
    }

    public RentalListViewModel(IItemService itemService)
    {
        _itemService = itemService;
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

            var result = await _itemService.GetIncomingRentalsAsync();

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

            var result = await _itemService.GetOutgoingRentalsAsync();

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