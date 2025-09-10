namespace Data;

public class Restaurant
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string TimeZoneId { get; set; } = "UTC";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public int OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;

    public ICollection<MetricValue> MetricValues { get; set; } = [];
    public ICollection<Revenue> Revenues { get; set; } = [];
    public ICollection<Recommendation> Recommendations { get; set; } = [];
}