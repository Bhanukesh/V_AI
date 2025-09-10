namespace Data;

public enum RecommendationType
{
    Performance,
    Revenue,
    Operations,
    Staffing,
    Cost
}

public enum RecommendationPriority
{
    Low,
    Medium,
    High,
    Critical
}

public class Recommendation
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public RecommendationType Type { get; set; }
    public RecommendationPriority Priority { get; set; }
    public decimal? EstimatedImpact { get; set; }
    public string? ActionItems { get; set; } // JSON string
    public bool IsImplemented { get; set; } = false;
    public DateTime? ImplementedAt { get; set; }
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }

    public int RestaurantId { get; set; }
    public Restaurant Restaurant { get; set; } = null!;
}