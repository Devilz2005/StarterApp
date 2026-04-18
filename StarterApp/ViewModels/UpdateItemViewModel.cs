using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Services;

namespace StarterApp.ViewModels;

[QueryProperty(nameof(ItemId), "itemId")]
public partial class UpdateItemViewModel : BaseViewModel
{
    private readonly IItemService _itemService;
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

    public UpdateItemViewModel()
    {
        Title = "Update Item";
    }

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
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            ClearError();

            var item = await _itemService.GetItemByIdAsync(id);

            if (item == null)
            {
                SetError("Item not found.");
                return;
            }

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
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task UpdateItemAsync()
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

        // Convert the DailyRate text into a decimal
        if (!decimal.TryParse(DailyRate, out var parsedRate) || parsedRate <= 0)
        {
            SetError("Enter a valid daily rate.");
            return;
        }

        // Convert location text → coordinates because API still expects lat/long
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

            var request = new UpdateItemRequest
            {
                Title = Title,
                Description = Description,
                DailyRate = parsedRate,

                // TEMP: still hardcoded until category selection is added
                CategoryId = 1,

                Latitude = latitude,
                Longitude = longitude
            };

            var result = await _itemService.UpdateItemAsync(ItemId, request);

            if (result.IsSuccess)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Success",
                    "Item updated successfully.",
                    "OK");

                await _navigationService.NavigateBackAsync();
            }
            else
            {
                SetError(result.Message);
            }
        }
        catch (Exception ex)
        {
            SetError($"Failed to update item: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        await _navigationService.NavigateBackAsync();
    }
}