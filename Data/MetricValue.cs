namespace Data;

public class MetricValue
{
    public long Id { get; set; }
    public string MetricName { get; set; } = null!;
    public decimal Value { get; set; }
    public DateTime Timestamp { get; set; }
    public string? Unit { get; set; }
    public string? Tags { get; set; } // JSON string for additional metadata
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int RestaurantId { get; set; }
    public Restaurant Restaurant { get; set; } = null!;
}