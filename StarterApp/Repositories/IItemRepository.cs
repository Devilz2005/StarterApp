using StarterApp.Services;

namespace StarterApp.Repositories;

// Repository interface for item and rental data access
// This defines what operations are available without showing how they are implemented
public interface IItemRepository
{
    // Gets all items
    Task<List<ItemDto>> GetAllAsync();

    // Gets a single item by its ID
    Task<ItemDto?> GetByIdAsync(int id);

    // Creates a new item
    Task<ServiceResult> CreateAsync(CreateItemRequest request);

    // Updates an existing item
    Task<ServiceResult> UpdateAsync(int id, UpdateItemRequest request);

    // Sends a rental request for an item
    Task<ServiceResult> RequestRentalAsync(int itemId);

    // Gets rental requests made by other users for this user's items
    Task<List<RentalDto>> GetIncomingRentalsAsync();

    // Gets rental requests made by the current user
    Task<List<RentalDto>> GetOutgoingRentalsAsync();
}