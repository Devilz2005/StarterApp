using StarterApp.Services;

namespace StarterApp.Repositories;

// Repository implementation that uses the API service underneath
public class ApiItemRepository : IItemRepository
{
    private readonly IItemService _itemService;

    public ApiItemRepository(IItemService itemService)
    {
        _itemService = itemService;
    }

    public Task<List<ItemDto>> GetAllAsync()
        => _itemService.GetItemsAsync();

    public Task<ItemDto?> GetByIdAsync(int id)
        => _itemService.GetItemByIdAsync(id);

    public Task<ServiceResult> CreateAsync(CreateItemRequest request)
        => _itemService.CreateItemAsync(request);

    public Task<ServiceResult> UpdateAsync(int id, UpdateItemRequest request)
        => _itemService.UpdateItemAsync(id, request);

    public Task<ServiceResult> RequestRentalAsync(int itemId)
        => _itemService.RequestRentalAsync(itemId);

    public Task<List<RentalDto>> GetIncomingRentalsAsync()
        => _itemService.GetIncomingRentalsAsync();

    public Task<List<RentalDto>> GetOutgoingRentalsAsync()
        => _itemService.GetOutgoingRentalsAsync();
}