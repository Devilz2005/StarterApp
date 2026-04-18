using System.Net.Http.Json;
using System.Text.Json;

namespace StarterApp.Services;

/// API-based implementation of IItemService.
/// This class talks directly to the shared REST API using HttpClient.
public class ApiItemService : IItemService
{
    private readonly HttpClient _httpClient;

    // Makes JSON property matching ignore case,
    // so camelCase API fields still map to PascalCase C# properties
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public ApiItemService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ItemDto>> GetItemsAsync()
    {
        // Send request manually so we can inspect failures
        var response = await _httpClient.GetAsync("items");

        // If API fails, throw the raw response text so TempViewModel can show it
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"GET /items failed ({(int)response.StatusCode}): {errorContent}");
        }

        // IMPORTANT:
        // The API does NOT return a raw List<ItemDto>
        // It returns: { "items": [ ... ] }
        // So we must deserialize into the wrapper class first
        var result = await response.Content.ReadFromJsonAsync<ItemListResponse>();

        // If null, return empty list to avoid crashes
        return result?.Items ?? new List<ItemDto>();
    }

    public async Task<ItemDto?> GetItemByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"items/{id}");

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"GET /items/{id} failed ({(int)response.StatusCode}): {errorContent}");
        }

        // This one works normally because API returns a single object
        return await response.Content.ReadFromJsonAsync<ItemDto>();
    }

    public async Task<ServiceResult> CreateItemAsync(CreateItemRequest request)
    {
        try
        {
            // Sends POST request with JSON body to create a new item
            var response = await _httpClient.PostAsJsonAsync("items", request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return new ServiceResult(false, $"Create failed: {errorContent}");
            }

            return new ServiceResult(true, "Item created successfully");
        }
        catch (Exception ex)
        {
            return new ServiceResult(false, $"Create failed: {ex.Message}");
        }
    }

    public async Task<ServiceResult> UpdateItemAsync(int id, UpdateItemRequest request)
    {
        try
        {
            // Sends PUT request to update an existing item
            var response = await _httpClient.PutAsJsonAsync($"items/{id}", request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return new ServiceResult(false, $"Update failed: {errorContent}");
            }

            return new ServiceResult(true, "Item updated successfully");
        }
        catch (Exception ex)
        {
            return new ServiceResult(false, $"Update failed: {ex.Message}");
        }
    }

    public async Task<ServiceResult> RequestRentalAsync(int itemId)
    {
        try
        {
            // Temporary test dates so the API gets everything it requires
            var requestBody = new
            {
                itemId = itemId,
                startDate = DateTime.UtcNow.Date.AddDays(1).ToString("yyyy-MM-dd"),
                endDate = DateTime.UtcNow.Date.AddDays(3).ToString("yyyy-MM-dd")
            };

            var response = await _httpClient.PostAsJsonAsync("rentals", requestBody);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return new ServiceResult(false, $"Request failed: {errorContent}");
            }

            return new ServiceResult(true, "Rental request sent successfully");
        }
        catch (Exception ex)
        {
            return new ServiceResult(false, $"Request failed: {ex.Message}");
        }
    }

    public async Task<List<RentalDto>> GetIncomingRentalsAsync()
    {
        var response = await _httpClient.GetAsync("rentals/incoming");

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Incoming rentals failed: {error}");
        }

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        // The rentals endpoints return a wrapper object:
        // { "rentals": [ ... ], "totalRentals": n }
        if (doc.RootElement.TryGetProperty("rentals", out var rentalsElement))
        {
            return JsonSerializer.Deserialize<List<RentalDto>>(
                rentalsElement.GetRawText(),
                _jsonOptions) ?? new List<RentalDto>();
        }

        return new List<RentalDto>();
    }

    public async Task<List<RentalDto>> GetOutgoingRentalsAsync()
    {
        var response = await _httpClient.GetAsync("rentals/outgoing");

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Outgoing rentals failed: {error}");
        }

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        // The rentals endpoints return a wrapper object:
        // { "rentals": [ ... ], "totalRentals": n }
        if (doc.RootElement.TryGetProperty("rentals", out var rentalsElement))
        {
            return JsonSerializer.Deserialize<List<RentalDto>>(
                rentalsElement.GetRawText(),
                _jsonOptions) ?? new List<RentalDto>();
        }

        return new List<RentalDto>();
    }
}