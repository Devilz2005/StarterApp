namespace StarterApp.Services;

/// Represents an item returned from the API.
public class ItemDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal DailyRate { get; set; }

    public string Category { get; set; } = string.Empty;

    public int? OwnerId { get; set; }

    public string? OwnerName { get; set; }

    public int CategoryId { get; set; }

    public double? OwnerRating { get; set; }

    public bool IsAvailable { get; set; }

    public double? AverageRating { get; set; }

    public DateTime CreatedAt { get; set; }

    // These are what the API is more likely returning
    public double? Latitude { get; set; }

    public double? Longitude { get; set; }

    // Convert known coordinates back into a readable place name for the UI
    public string Location =>
        (Latitude, Longitude) switch
        {
            (55.9533, -3.1883) => "Edinburgh",
            (55.8642, -4.2518) => "Glasgow",
            (57.1497, -2.0943) => "Aberdeen",
            (56.4620, -2.9707) => "Dundee",
            _ when Latitude.HasValue && Longitude.HasValue => $"{Latitude:F4}, {Longitude:F4}",
            _ => string.Empty
        };
}

/// Represents the data sent when creating a new item.
public class CreateItemRequest
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal DailyRate { get; set; }

    public int CategoryId { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }
}

/// Represents the data sent when updating an item.
public class UpdateItemRequest
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal DailyRate { get; set; }

    public int CategoryId { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }
}

/// Result object for service responses.
public class ServiceResult
{
    public bool IsSuccess { get; }

    public string Message { get; }

    public ServiceResult(bool isSuccess, string message)
    {
        IsSuccess = isSuccess;
        Message = message;
    }
}

// The GET /items endpoint returns { "items": [ ... ] }
public class ItemListResponse
{
    public List<ItemDto> Items { get; set; } = new();
}

public class RentalDto
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public string ItemTitle { get; set; } = string.Empty;

    public int OwnerId { get; set; }
    public string OwnerName { get; set; } = string.Empty;
    public double? OwnerRating { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public string Status { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }

    public DateTime RequestedAt { get; set; }
}