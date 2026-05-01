using System.Net.Http.Json;
using System.Text.Json;

namespace StarterApp.Services;

// API-based implementation of IItemService.
// This class communicates with the shared REST API using HttpClient.
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
        // Sends a GET request to retrieve all items
        var response = await _httpClient.GetAsync("items");

        // If the API request fails, throw an exception with the error details
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"GET /items failed ({(int)response.StatusCode}): {errorContent}");
        }

        // The API returns an object that contains the item list
        var result = await response.Content.ReadFromJsonAsync<ItemListResponse>();

        // If the result is null, return an empty list to prevent crashes
        return result?.Items ?? new List<ItemDto>();
    }

    public async Task<ItemDto?> GetItemByIdAsync(int id)
    {
        // Sends a GET request to retrieve a single item by ID
        var response = await _httpClient.GetAsync($"items/{id}");

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"GET /items/{id} failed ({(int)response.StatusCode}): {errorContent}");
        }

        // This endpoint returns a single item object
        return await response.Content.ReadFromJsonAsync<ItemDto>();
    }

    public async Task<ServiceResult> CreateItemAsync(CreateItemRequest request)
    {
        try
        {
            // Sends a POST request with item data to create a new item
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
            // Returns error information instead of throwing an exception
            return new ServiceResult(false, $"Create failed: {ex.Message}");
        }
    }

    public async Task<ServiceResult> UpdateItemAsync(int id, UpdateItemRequest request)
    {
        try
        {
            // Sends a PUT request to update an existing item
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
            // Returns error information instead of throwing an exception
            return new ServiceResult(false, $"Update failed: {ex.Message}");
        }
    }

    public async Task<ServiceResult> RequestRentalAsync(int itemId)
    {
        try
        {
            // Creates the rental request data required by the API
            var requestBody = new
            {
                itemId = itemId,
                startDate = DateTime.UtcNow.Date.AddDays(1).ToString("yyyy-MM-dd"),
                endDate = DateTime.UtcNow.Date.AddDays(3).ToString("yyyy-MM-dd")
            };

            // Sends the rental request to the API
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
            // Returns error information instead of throwing an exception
            return new ServiceResult(false, $"Request failed: {ex.Message}");
        }
    }

    public async Task<List<RentalDto>> GetIncomingRentalsAsync()
    {
        // Sends a request to get incoming rental requests
        var response = await _httpClient.GetAsync("rentals/incoming");

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Incoming rentals failed: {error}");
        }

        // Reads the raw JSON response
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        // The API returns a wrapper object containing the rentals array
        if (doc.RootElement.TryGetProperty("rentals", out var rentalsElement))
        {
            return JsonSerializer.Deserialize<List<RentalDto>>(
                rentalsElement.GetRawText(),
                _jsonOptions) ?? new List<RentalDto>();
        }

        // Returns an empty list if no rentals are found
        return new List<RentalDto>();
    }

    public async Task<List<RentalDto>> GetOutgoingRentalsAsync()
    {
        // Sends a request to get outgoing rental requests
        var response = await _httpClient.GetAsync("rentals/outgoing");

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Outgoing rentals failed: {error}");
        }

        // Reads the raw JSON response
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        // The API returns a wrapper object containing the rentals array
        if (doc.RootElement.TryGetProperty("rentals", out var rentalsElement))
        {
            return JsonSerializer.Deserialize<List<RentalDto>>(
                rentalsElement.GetRawText(),
                _jsonOptions) ?? new List<RentalDto>();
        }

        // Returns an empty list if no rentals are found
        return new List<RentalDto>();
    }
}