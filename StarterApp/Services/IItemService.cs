namespace StarterApp.Services;

// Defines the item-related operations the rest of the app can call.
// This is the interface, so ViewModels do not need to know how the data is actually fetched or saved
public interface IItemService
{
    // Gets all items from the API.
    Task<List<ItemDto>> GetItemsAsync();

    // Gets one specific item by its ID.
    Task<ItemDto?> GetItemByIdAsync(int id);

    // Sends a request to create a new item.
    Task<ServiceResult> CreateItemAsync(CreateItemRequest request);

    // Sends a request to update an existing item.
    Task<ServiceResult> UpdateItemAsync(int id, UpdateItemRequest request);

    // Sends a rental request for an item
    Task<ServiceResult> RequestRentalAsync(int itemId);

    Task<List<RentalDto>> GetIncomingRentalsAsync();
    Task<List<RentalDto>> GetOutgoingRentalsAsync();

}