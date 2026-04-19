using StarterApp.Services;

namespace StarterApp.Repositories;

// Repository interface for item and rental data access
public interface IItemRepository
{
    Task<List<ItemDto>> GetAllAsync();
    Task<ItemDto?> GetByIdAsync(int id);
    Task<ServiceResult> CreateAsync(CreateItemRequest request);
    Task<ServiceResult> UpdateAsync(int id, UpdateItemRequest request);
    Task<ServiceResult> RequestRentalAsync(int itemId);
    Task<List<RentalDto>> GetIncomingRentalsAsync();
    Task<List<RentalDto>> GetOutgoingRentalsAsync();
}