using StarterApp.Services;

namespace StarterApp.Repositories;

// Repository implementation that uses the API service underneath
// This acts as a middle layer between ViewModels and the API service
public class ApiItemRepository : IItemRepository
{
    // Service that actually communicates with the API
    private readonly IItemService _itemService;

    // Constructor receives the API service through dependency injection
    public ApiItemRepository(IItemService itemService)
    {
        _itemService = itemService;
    }

    // Gets all items by calling the API service
    public Task<List<ItemDto>> GetAllAsync()
        => _itemService.GetItemsAsync();

    // Gets a single item by ID
    public Task<ItemDto?> GetByIdAsync(int id)
        => _itemService.GetItemByIdAsync(id);

    // Creates a new item by sending data to the API
    public Task<ServiceResult> CreateAsync(CreateItemRequest request)
        => _itemService.CreateItemAsync(request);

    // Updates an existing item
    public Task<ServiceResult> UpdateAsync(int id, UpdateItemRequest request)
        => _itemService.UpdateItemAsync(id, request);

    // Sends a rental request for an item
    public Task<ServiceResult> RequestRentalAsync(int itemId)
        => _itemService.RequestRentalAsync(itemId);

    // Gets rental requests made by other users for this user's items
    public Task<List<RentalDto>> GetIncomingRentalsAsync()
        => _itemService.GetIncomingRentalsAsync();

    // Gets rental requests made by the current user
    public Task<List<RentalDto>> GetOutgoingRentalsAsync()
        => _itemService.GetOutgoingRentalsAsync();
}