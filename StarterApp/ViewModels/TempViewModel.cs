/// @file TempViewModel.cs
/// @brief Temporary placeholder view model (now used for item management testing)
/// @author StarterApp Development Team
/// @date 2025

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Services;
using System.Collections.ObjectModel;

namespace StarterApp.ViewModels;

/// @brief Temporary view model for placeholder pages
/// @details Now extended to load and display items from the API
/// @extends ObservableObject
public partial class TempViewModel : ObservableObject
{
    /// @brief Item service for communicating with the API
    private readonly IItemService _itemService;

    /// @brief Collection of items to display in the UI
    /// @details ObservableCollection automatically updates the UI when changed
    [ObservableProperty]
    private ObservableCollection<ItemDto> items = new();

    /// @brief Indicates whether items are currently being loaded
    /// @details Used to prevent duplicate requests and show loading state
    [ObservableProperty]
    private bool isLoading;

    /// @brief Stores any error message from API calls
    /// @details Displayed in the UI if something goes wrong
    [ObservableProperty]
    private string errorMessage = string.Empty;

    /// @brief Initializes a new instance of the TempViewModel class
    /// @param itemService The item service used to fetch data from the API
    public TempViewModel(IItemService itemService)
    {
        _itemService = itemService;

        // We inject the service so this ViewModel doesn't need to know
        // how API calls actually work (clean separation of concerns)
    }

    /// @brief Loads all items from the API
    /// @details Calls the item service and updates the Items collection
    /// @return A task representing the asynchronous operation
    [RelayCommand]
    private async Task LoadItemsAsync()
    {
        // Prevent multiple calls if already loading
        if (IsLoading)
            return;

        try
        {
            IsLoading = true;

            // Clear any previous error before starting
            ErrorMessage = string.Empty;

            // Call the API through the service layer
            var result = await _itemService.GetItemsAsync();

            // Clear old data so we don’t duplicate items
            Items.Clear();

            // Add each item into the observable list (this updates UI automatically)
            foreach (var item in result)
            {
                Items.Add(item);
            }
        }
        catch (Exception ex)
        {
            // Store the error so the UI can show it
            ErrorMessage = $"Failed to load items: {ex.Message}";
        }
        finally
        {
            // Always reset loading state
            IsLoading = false;
        }
    }
}