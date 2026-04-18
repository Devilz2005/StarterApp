using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StarterApp.Services;

namespace StarterApp.ViewModels;

[QueryProperty(nameof(ItemId), "itemId")]
public partial class ItemDetailViewModel : BaseViewModel
{
    private readonly IItemService _itemService;
    private readonly INavigationService _navigationService;

    // Stores the ID passed through navigation
    [ObservableProperty]
    private int itemId;

    // Full item loaded from the API
    [ObservableProperty]
    private ItemDto? selectedItem;

    public ItemDetailViewModel()
    {
        Title = "Item Details";
    }

    public ItemDetailViewModel(IItemService itemService, INavigationService navigationService)
    {
        _itemService = itemService;
        _navigationService = navigationService;
        Title = "Item Details";
    }

    partial void OnItemIdChanged(int value)
    {
        // When Shell passes itemId in the route,
        // automatically load that item's details
        _ = LoadItemAsync(value);
    }

    // Lets the page force a refresh when it appears again
    public async Task RefreshAsync()
    {
        if (ItemId > 0)
        {
            await LoadItemAsync(ItemId);
        }
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

            SelectedItem = item;

            // Update page title to match the item
            Title = item.Title;
        }
        catch (Exception ex)
        {
            SetError($"Failed to load item details: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    // Opens the update page for the current item
    [RelayCommand]
    private async Task NavigateToUpdateItemAsync()
    {
        if (SelectedItem == null)
            return;

        await _navigationService.NavigateToAsync($"{nameof(Views.UpdateItemPage)}?itemId={SelectedItem.Id}");
    }

    // Sends a rental request for this item
[RelayCommand]
private async Task RequestRentalAsync()
{
    if (SelectedItem == null)
        return;

    var result = await _itemService.RequestRentalAsync(SelectedItem.Id);

    if (result.IsSuccess)
    {
        await Application.Current.MainPage.DisplayAlert(
            "Success",
            "Rental request sent.",
            "OK");
    }
    else
    {
        SetError(result.Message);
    }
}
}