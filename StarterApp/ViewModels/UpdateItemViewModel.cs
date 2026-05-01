using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Services;

namespace StarterApp.ViewModels;

// ViewModel responsible for loading and updating an existing item
[QueryProperty(nameof(ItemId), "itemId")]
public partial class UpdateItemViewModel : BaseViewModel
{
    // Service used to get and update item data through the API
    private readonly IItemService _itemService;

    // Service used to move between pages
    private readonly INavigationService _navigationService;

    // The item ID passed in through navigation
    [ObservableProperty]
    private int itemId;

    // Form fields bound to the UI
    [ObservableProperty]
    private string title = string.Empty;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private string dailyRate = string.Empty;

    [ObservableProperty]
    private string category = string.Empty;

    [ObservableProperty]
    private string location = string.Empty;

    // Default constructor
    public UpdateItemViewModel()
    {
        Title = "Update Item";
    }

    // Main constructor with dependency injection
    public UpdateItemViewModel(IItemService itemService, INavigationService navigationService)
    {
        _itemService = itemService;
        _navigationService = navigationService;
        Title = "Update Item";
    }

    // When Shell passes itemId, load the item and prefill the form
    partial void OnItemIdChanged(int value)
    {
        _ = LoadItemAsync(value);
    }

    private async Task LoadItemAsync(int id)
    {
        // Prevent multiple loads at the same time
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            ClearError();

            // Get the selected item from the API
            var item = await _itemService.GetItemByIdAsync(id);

            if (item == null)
            {
                SetError("Item not found.");
                return;
            }

            // Fill the form with the existing item details
            Title = item.Title;
            Description = item.Description;
            DailyRate = item.DailyRate.ToString();
            Category = item.Category;
            Location = item.Location ?? string.Empty;
        }
        catch (Exception ex)
        {
            SetError($"Failed to load item: {ex.Message}");
        }
        finally
        {
            // Reset loading state
            IsBusy = false;
        }
    }

    // Command triggered when the user presses the "Update" button
    [RelayCommand]
    private async Task UpdateItemAsync()
    {
        // Prevent multiple update requests at the same time
        if (IsBusy)
            return;

        // Make sure user filled everything in
        if (string.IsNullOrWhiteSpace(Title) ||
            string.IsNullOrWhiteSpace(Description) ||
            string.IsNullOrWhiteSpace(DailyRate) ||
            string.IsNullOrWhiteSpace(Category) ||
            string.IsNullOrWhiteSpace(Location))
        {
            SetError("Please fill in all fields.");
            return;
        }

        // Convert the DailyRate text into a decimal
        if (!decimal.TryParse(DailyRate, out var parsedRate) || parsedRate <= 0)
        {
            SetError("Enter a valid daily rate.");
            return;
        }

        // Convert location text into coordinates because the API expects latitude and longitude
        (double latitude, double longitude) = Location.Trim().ToLower() switch
        {
            "edinburgh" => (55.9533, -3.1883),
            "glasgow" => (55.8642, -4.2518),
            "aberdeen" => (57.1497, -2.0943),
            "dundee" => (56.4620, -2.9707),
            _ => (55.9533, -3.1883)
        };

        try
        {
            IsBusy = true;
            ClearError();

            // Create the request object that matches the API format
            var request = new UpdateItemRequest
            {
                Title = Title,
                Description = Description,
                DailyRate = parsedRate,

                // Temporary fixed category ID until category selection is added
                CategoryId = 1,

                Latitude = latitude,
                Longitude = longitude
            };

            // Send the updated item details to the API
            var result = await _itemService.UpdateItemAsync(ItemId, request);

            if (result.IsSuccess)
            {
                // Show success message
                await Application.Current.MainPage.DisplayAlert(
                    "Success",
                    "Item updated successfully.",
                    "OK");

                // Navigate back to the previous page
                await _navigationService.NavigateBackAsync();
            }
            else
            {
                // Show any API error message
                SetError(result.Message);
            }
        }
        catch (Exception ex)
        {
            // Handle unexpected errors
            SetError($"Failed to update item: {ex.Message}");
        }
        finally
        {
            // Reset loading state
            IsBusy = false;
        }
    }

    // Command triggered when the user presses the "Cancel" button
    [RelayCommand]
    private async Task CancelAsync()
    {
        // Navigate back without saving changes
        await _navigationService.NavigateBackAsync();
    }
}