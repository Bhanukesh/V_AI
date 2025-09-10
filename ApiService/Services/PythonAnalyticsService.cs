using System.Text;
using System.Text.Json;

namespace ApiService.Services;

public interface IPythonAnalyticsService
{
    Task<CorrelationAnalysisResponse> CalculateCorrelationsAsync(int restaurantId, CancellationToken cancellationToken = default);
    Task<ForecastResponse> GenerateForecastAsync(int restaurantId, CancellationToken cancellationToken = default);
}

public class PythonAnalyticsService : IPythonAnalyticsService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PythonAnalyticsService> _logger;

    public PythonAnalyticsService(HttpClient httpClient, ILogger<PythonAnalyticsService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<CorrelationAnalysisResponse> CalculateCorrelationsAsync(int restaurantId, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new CorrelationRequest
            {
                RestaurantId = restaurantId,
                Metrics = new[] { "prep_time", "table_turnover", "order_accuracy", "customer_satisfaction", "wait_time" },
                CorrelationType = "pearson"
            };

            var json = JsonSerializer.Serialize(request, JsonSerializerOptions.Web);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogInformation("Calling Python analytics service for restaurant {RestaurantId}", restaurantId);

            var response = await _httpClient.PostAsync("/analytics/correlation", content, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<CorrelationAnalysisResponse>(responseJson, JsonSerializerOptions.Web);

            _logger.LogInformation("Successfully received correlation analysis for restaurant {RestaurantId}", restaurantId);
            return result ?? new CorrelationAnalysisResponse { Correlations = new List<CorrelationResult>() };
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP error calling Python analytics service for restaurant {RestaurantId}", restaurantId);
            throw new InvalidOperationException("Analytics service unavailable", ex);
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            _logger.LogError(ex, "Timeout calling Python analytics service for restaurant {RestaurantId}", restaurantId);
            throw new InvalidOperationException("Analytics service timeout", ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "JSON deserialization error from Python analytics service for restaurant {RestaurantId}", restaurantId);
            throw new InvalidOperationException("Invalid response from analytics service", ex);
        }
    }

    public async Task<ForecastResponse> GenerateForecastAsync(int restaurantId, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new ForecastRequest
            {
                RestaurantId = restaurantId,
                DaysToForecast = 30
            };

            var json = JsonSerializer.Serialize(request, JsonSerializerOptions.Web);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _logger.LogInformation("Calling Python analytics service for forecast of restaurant {RestaurantId}", restaurantId);

            var response = await _httpClient.PostAsync("/analytics/forecast", content, cancellationToken);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<ForecastResponse>(responseJson, JsonSerializerOptions.Web);

            _logger.LogInformation("Successfully received forecast for restaurant {RestaurantId}", restaurantId);
            return result ?? new ForecastResponse { Forecast = new List<ForecastPoint>() };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Python analytics service for forecast of restaurant {RestaurantId}", restaurantId);
            throw new InvalidOperationException("Forecast service unavailable", ex);
        }
    }
}

// DTOs for Python service communication
public class CorrelationRequest
{
    public int RestaurantId { get; set; }
    public string[] Metrics { get; set; } = Array.Empty<string>();
    public string CorrelationType { get; set; } = "pearson";
}

public class CorrelationAnalysisResponse
{
    public List<CorrelationResult> Correlations { get; set; } = new();
    public string AnalysisDate { get; set; } = DateTime.UtcNow.ToString("O");
    public string CorrelationType { get; set; } = "pearson";
}

public class CorrelationResult
{
    public string MetricName { get; set; } = "";
    public double Coefficient { get; set; }
    public double PValue { get; set; }
    public bool IsSignificant { get; set; }
    public string Interpretation { get; set; } = "";
}

public class ForecastRequest
{
    public int RestaurantId { get; set; }
    public int DaysToForecast { get; set; } = 30;
}

public class ForecastResponse
{
    public List<ForecastPoint> Forecast { get; set; } = new();
    public double ConfidenceInterval { get; set; } = 0.95;
    public string Model { get; set; } = "";
}

public class ForecastPoint
{
    public string Date { get; set; } = "";
    public double PredictedRevenue { get; set; }
    public double LowerBound { get; set; }
    public double UpperBound { get; set; }
}