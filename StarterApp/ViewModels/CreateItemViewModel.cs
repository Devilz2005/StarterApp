using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Services;

namespace StarterApp.ViewModels;

public partial class CreateItemViewModel : BaseViewModel
{
    private readonly IItemService _itemService;
    private readonly INavigationService _navigationService;

    // These are bound to the input fields in the UI
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

    public CreateItemViewModel()
    {
        Title = "Create Item";
    }

    public CreateItemViewModel(IItemService itemService, INavigationService navigationService)
    {
        _itemService = itemService;
        _navigationService = navigationService;
        Title = "Create Item";
    }

    [RelayCommand]
    private async Task CreateItemAsync()
    {
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

        // Convert the DailyRate (string from UI) into a decimal
        if (!decimal.TryParse(DailyRate, out var parsedRate) || parsedRate <= 0)
        {
            SetError("Enter a valid daily rate.");
            return;
        }

        // Convert location text → coordinates (because API requires lat/long)
        (double latitude, double longitude) = Location.Trim().ToLower() switch
        {
            "edinburgh" => (55.9533, -3.1883),
            "glasgow" => (55.8642, -4.2518),
            "aberdeen" => (57.1497, -2.0943),
            "dundee" => (56.4620, -2.9707),

            // fallback so it doesn't crash if user types something random
            _ => (55.9533, -3.1883)
        };

        try
        {
            IsBusy = true;
            ClearError();

            // Build the object that matches what the API expects
            var request = new CreateItemRequest
            {
                Title = Title,
                Description = Description,
                DailyRate = parsedRate,

                // TEMP: hardcoded until we add real category selection
                CategoryId = 1,

                Latitude = latitude,
                Longitude = longitude
            };

            // Send request to API
            var result = await _itemService.CreateItemAsync(request);

            if (result.IsSuccess)
            {
                // Show success popup
                await Application.Current.MainPage.DisplayAlert(
                    "Success",
                    "Item created successfully.",
                    "OK");

                // Go back to item list page
                await _navigationService.NavigateBackAsync();
            }
            else
            {
                // Show API error message
                SetError(result.Message);
            }
        }
        catch (Exception ex)
        {
            // Catch any unexpected crashes
            SetError($"Failed to create item: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        // Just go back without doing anything
        await _navigationService.NavigateBackAsync();
    }
}