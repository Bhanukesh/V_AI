namespace ApiService.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Analytics.Commands;
using Restaurant.DTO;
using ApiService.Services;
using Microsoft.EntityFrameworkCore;
using Data;

[ApiController]
[Route("api/[controller]")]
public class AnalyticsController(IMediator mediator, IPythonAnalyticsService analyticsService, RestaurantDbContext context, ILogger<AnalyticsController> logger) : ControllerBase
{
    [HttpPost("correlation", Name = nameof(CalculateCorrelation))]
    public async Task<ActionResult<IEnumerable<CorrelationResultDto>>> CalculateCorrelation(CorrelationRequestDto request)
    {
        try
        {
            if (request.MetricNames.Length < 2)
            {
                return BadRequest("At least two metrics are required for correlation analysis.");
            }

            var results = await mediator.Send(new CalculateCorrelationCommand(request));
            return Ok(results);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Get correlation analysis between restaurant metrics and revenue via Python service
    /// </summary>
    [HttpGet("restaurants/{restaurantId}/correlations", Name = nameof(GetCorrelationAnalysis))]
    public async Task<ActionResult<CorrelationAnalysisDto>> GetCorrelationAnalysis(
        int restaurantId, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Verify restaurant exists
            var restaurant = await context.Restaurants.FindAsync(restaurantId);
            if (restaurant == null)
            {
                return NotFound($"Restaurant with ID {restaurantId} not found");
            }

            logger.LogInformation("Starting correlation analysis for restaurant {RestaurantId}", restaurantId);

            // Call Python analytics service
            var correlationResponse = await analyticsService.CalculateCorrelationsAsync(restaurantId, cancellationToken);

            // Convert to API response format
            var result = new CorrelationAnalysisDto
            {
                RestaurantId = restaurantId,
                RestaurantName = restaurant.Name,
                AnalysisDate = DateTime.UtcNow,
                CorrelationType = correlationResponse.CorrelationType,
                Correlations = correlationResponse.Correlations.Select(c => new CorrelationDto
                {
                    MetricName = c.MetricName,
                    DisplayName = GetMetricDisplayName(c.MetricName),
                    Coefficient = Math.Round(c.Coefficient, 3),
                    PValue = Math.Round(c.PValue, 4),
                    IsSignificant = c.PValue < 0.05,
                    Strength = GetCorrelationStrength(Math.Abs(c.Coefficient)),
                    Direction = c.Coefficient > 0 ? "Positive" : "Negative",
                    Interpretation = GenerateInterpretation(c.MetricName, c.Coefficient, c.PValue)
                }).OrderByDescending(c => Math.Abs(c.Coefficient)).ToList()
            };

            logger.LogInformation("Successfully completed correlation analysis for restaurant {RestaurantId}", restaurantId);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex, "Analytics service error for restaurant {RestaurantId}", restaurantId);
            return StatusCode(503, new { error = "Analytics service unavailable", message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during correlation analysis for restaurant {RestaurantId}", restaurantId);
            return StatusCode(500, new { error = "Internal server error", message = "An unexpected error occurred" });
        }
    }

    /// <summary>
    /// Get basic analytics data for correlation analysis
    /// </summary>
    [HttpGet("restaurants/{restaurantId}/data", Name = nameof(GetAnalyticsData))]
    public async Task<ActionResult<AnalyticsDataDto>> GetAnalyticsData(
        int restaurantId,
        [FromQuery] int days = 90,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var restaurant = await context.Restaurants.FindAsync(restaurantId);
            if (restaurant == null)
            {
                return NotFound($"Restaurant with ID {restaurantId} not found");
            }

            var startDate = DateTime.UtcNow.AddDays(-days);

            // Get revenue data
            var revenueData = await context.Revenues
                .Where(r => r.RestaurantId == restaurantId && r.CreatedAt >= startDate)
                .Select(r => new { r.Date, r.TotalRevenue })
                .ToListAsync(cancellationToken);

            // Get metrics data
            var metricsData = await context.MetricValues
                .Where(m => m.RestaurantId == restaurantId && m.Timestamp >= startDate)
                .GroupBy(m => new { m.MetricName, Date = m.Timestamp.Date })
                .Select(g => new
                {
                    g.Key.MetricName,
                    g.Key.Date,
                    AverageValue = g.Average(m => m.Value)
                })
                .ToListAsync(cancellationToken);

            var result = new AnalyticsDataDto
            {
                RestaurantId = restaurantId,
                RestaurantName = restaurant.Name,
                DataPeriod = $"{days} days",
                RevenueDataPoints = revenueData.Count,
                MetricDataPoints = metricsData.Count,
                AvailableMetrics = metricsData.Select(m => m.MetricName).Distinct().ToList(),
                DataQuality = revenueData.Count > 30 && metricsData.Count > 100 ? "Good" : "Limited"
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving analytics data for restaurant {RestaurantId}", restaurantId);
            return StatusCode(500, new { error = "Data retrieval failed", message = ex.Message });
        }
    }

    private static string GetMetricDisplayName(string metricName) => metricName switch
    {
        "prep_time" => "Kitchen Prep Time",
        "table_turnover" => "Table Turnover Time", 
        "order_accuracy" => "Order Accuracy",
        "customer_satisfaction" => "Customer Satisfaction",
        "wait_time" => "Customer Wait Time",
        _ => metricName.Replace("_", " ").ToTitleCase()
    };

    private static string GetCorrelationStrength(double coefficient) => Math.Abs(coefficient) switch
    {
        >= 0.7 => "Strong",
        >= 0.5 => "Moderate",
        >= 0.3 => "Weak",
        _ => "Very Weak"
    };

    private static string GenerateInterpretation(string metricName, double coefficient, double pValue)
    {
        var direction = coefficient > 0 ? "increases" : "decreases";
        var significance = pValue < 0.05 ? "significantly" : "not significantly";
        var strength = GetCorrelationStrength(Math.Abs(coefficient)).ToLower();
        
        return $"Revenue {direction} with {GetMetricDisplayName(metricName)} ({strength} correlation, {significance} correlated)";
    }
}

// DTOs for new endpoints
public class CorrelationAnalysisDto
{
    public int RestaurantId { get; set; }
    public string RestaurantName { get; set; } = "";
    public DateTime AnalysisDate { get; set; }
    public string CorrelationType { get; set; } = "";
    public List<CorrelationDto> Correlations { get; set; } = new();
}

public class CorrelationDto
{
    public string MetricName { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public double Coefficient { get; set; }
    public double PValue { get; set; }
    public bool IsSignificant { get; set; }
    public string Strength { get; set; } = "";
    public string Direction { get; set; } = "";
    public string Interpretation { get; set; } = "";
}

public class AnalyticsDataDto
{
    public int RestaurantId { get; set; }
    public string RestaurantName { get; set; } = "";
    public string DataPeriod { get; set; } = "";
    public int RevenueDataPoints { get; set; }
    public int MetricDataPoints { get; set; }
    public List<string> AvailableMetrics { get; set; } = new();
    public string DataQuality { get; set; } = "";
}

// Extension method for title case
public static class StringExtensions
{
    public static string ToTitleCase(this string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());
    }
}