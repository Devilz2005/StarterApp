// View model for displaying and loading items
// Loads and displays items from the API
// Extends ObservableObject
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Services;
using System.Collections.ObjectModel;

namespace StarterApp.ViewModels;

public partial class ItemListViewModel : ObservableObject
{
    // Item service for communicating with the API
    private readonly IItemService _itemService;

    // Collection of items to display in the UI
    // ObservableCollection automatically updates the UI when changed
    [ObservableProperty]
    private ObservableCollection<ItemDto> items = new();

    // Indicates whether items are currently being loaded
    // Used to prevent duplicate requests and show loading state
    [ObservableProperty]
    private bool isLoading;

    // Stores any error message from API calls
    // Displayed in the UI if something goes wrong
    [ObservableProperty]
    private string errorMessage = string.Empty;

    // Initializes a new instance of the ItemListViewModel class
    // itemService is used to fetch data from the API
    public ItemListViewModel(IItemService itemService)
    {
        _itemService = itemService;

        // Service is injected so the ViewModel doesn't handle HTTP directly
    }

    // Loads all items from the API
    // Calls the item service and updates the Items collection
    [RelayCommand]
    private async Task LoadItemsAsync()
    {
        // Prevent duplicate requests while already loading
        if (IsLoading)
            return;

        try
        {
            IsLoading = true;

            // Clear any previous error before starting a new request
            ErrorMessage = string.Empty;

            // Call the service to get the latest list of items
            var result = await _itemService.GetItemsAsync();

            // Clear existing items so the list doesn't duplicate
            Items.Clear();

            // Add each returned item to the observable collection
            foreach (var item in result)
            {
                Items.Add(item);
            }
        }
        catch (Exception ex)
        {
            // Show any loading error on the page
            ErrorMessage = $"Failed to load items: {ex.Message}";
        }
        finally
        {
            // Always reset loading state
            IsLoading = false;
        }
    }

    // Navigates to the create item page
    // Opens the form used to add a new item listing
    [RelayCommand]
    private async Task NavigateToCreateItemAsync()
    {
        await Shell.Current.GoToAsync(nameof(Views.CreateItemPage));
    }

    // Navigates to the rental requests page
    [RelayCommand]
    private async Task NavigateToRentalListAsync()
    {
        await Shell.Current.GoToAsync(nameof(Views.RentalListPage));
    }

    // Opens the detail page for the tapped item
    [RelayCommand]
    private async Task OpenItemDetailAsync(ItemDto? selectedItem)
    {
        if (selectedItem == null)
            return;

        await Shell.Current.GoToAsync($"{nameof(Views.ItemDetailPage)}?itemId={selectedItem.Id}");
    }
}