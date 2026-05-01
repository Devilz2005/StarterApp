using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Services;

namespace StarterApp.ViewModels;

// ViewModel responsible for handling the Create Item page logic
public partial class CreateItemViewModel : BaseViewModel
{
    // Service used to send item data to the API
    private readonly IItemService _itemService;

    // Service used for navigation between pages
    private readonly INavigationService _navigationService;

    // These properties are linked (bound) to the input fields in the UI
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

    // Default constructor (used in some cases like design-time or fallback)
    public CreateItemViewModel()
    {
        Title = "Create Item";
    }

    // Main constructor with dependency injection
    public CreateItemViewModel(IItemService itemService, INavigationService navigationService)
    {
        _itemService = itemService;
        _navigationService = navigationService;
        Title = "Create Item";
    }

    // Command triggered when the user presses the "Create" button
    [RelayCommand]
    private async Task CreateItemAsync()
    {
        // Prevent multiple requests at the same time
        if (IsBusy)
            return;

        // Check if any required field is empty
        if (string.IsNullOrWhiteSpace(Title) ||
            string.IsNullOrWhiteSpace(Description) ||
            string.IsNullOrWhiteSpace(DailyRate) ||
            string.IsNullOrWhiteSpace(Category) ||
            string.IsNullOrWhiteSpace(Location))
        {
            SetError("Please fill in all fields.");
            return;
        }

        // Convert the DailyRate from string to decimal
        if (!decimal.TryParse(DailyRate, out var parsedRate) || parsedRate <= 0)
        {
            SetError("Enter a valid daily rate.");
            return;
        }

        // Convert the location text into latitude and longitude coordinates
        // The API requires coordinates instead of plain text
        (double latitude, double longitude) = Location.Trim().ToLower() switch
        {
            "edinburgh" => (55.9533, -3.1883),
            "glasgow" => (55.8642, -4.2518),
            "aberdeen" => (57.1497, -2.0943),
            "dundee" => (56.4620, -2.9707),

            // Default fallback location to prevent errors
            _ => (55.9533, -3.1883)
        };

        try
        {
            IsBusy = true;
            ClearError();

            // Create the request object that matches the API format
            var request = new CreateItemRequest
            {
                Title = Title,
                Description = Description,
                DailyRate = parsedRate,

                // fixed category ID
                CategoryId = 1,

                Latitude = latitude,
                Longitude = longitude
            };

            // Send the request to the API
            var result = await _itemService.CreateItemAsync(request);

            if (result.IsSuccess)
            {
                // Show a success message to the user
                await Application.Current.MainPage.DisplayAlert(
                    "Success",
                    "Item created successfully.",
                    "OK");

                // Navigate back to the previous page (item list)
                await _navigationService.NavigateBackAsync();
            }
            else
            {
                // Show any error returned by the API
                SetError(result.Message);
            }
        }
        catch (Exception ex)
        {
            // Handle unexpected errors
            SetError($"Failed to create item: {ex.Message}");
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
        // Simply navigate back without saving anything
        await _navigationService.NavigateBackAsync();
    }
}