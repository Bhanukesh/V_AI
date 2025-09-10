namespace Data;

public class Revenue
{
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal FoodRevenue { get; set; }
    public decimal BeverageRevenue { get; set; }
    public int TransactionCount { get; set; }
    public decimal AverageTicket { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public int RestaurantId { get; set; }
    public Restaurant Restaurant { get; set; } = null!;
}