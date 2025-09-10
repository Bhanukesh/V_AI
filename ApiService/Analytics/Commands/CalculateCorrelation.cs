namespace ApiService.Analytics.Commands;

using MediatR;
using Data;
using Restaurant.DTO;
using Microsoft.EntityFrameworkCore;

public record CalculateCorrelationCommand(CorrelationRequestDto Request) : IRequest<IEnumerable<CorrelationResultDto>>;

public class CalculateCorrelationHandler(RestaurantDbContext context) : IRequestHandler<CalculateCorrelationCommand, IEnumerable<CorrelationResultDto>>
{
    public async Task<IEnumerable<CorrelationResultDto>> Handle(CalculateCorrelationCommand request, CancellationToken cancellationToken)
    {
        var req = request.Request;
        var results = new List<CorrelationResultDto>();

        // Get all metric data for the restaurant and time period
        var allMetrics = await context.MetricValues
            .Where(m => m.RestaurantId == req.RestaurantId &&
                       m.Timestamp >= req.StartDate &&
                       m.Timestamp <= req.EndDate &&
                       req.MetricNames.Contains(m.MetricName))
            .ToListAsync(cancellationToken);

        // Group by metric name and aggregate by hour to ensure consistent time series
        var metricsByName = allMetrics
            .GroupBy(m => m.MetricName)
            .ToDictionary(
                g => g.Key,
                g => g.GroupBy(m => new DateTime(m.Timestamp.Year, m.Timestamp.Month, m.Timestamp.Day, m.Timestamp.Hour, 0, 0))
                     .ToDictionary(tg => tg.Key, tg => (double)tg.Average(m => m.Value))
            );

        // Calculate correlations for all pairs
        for (int i = 0; i < req.MetricNames.Length; i++)
        {
            for (int j = i + 1; j < req.MetricNames.Length; j++)
            {
                var metric1 = req.MetricNames[i];
                var metric2 = req.MetricNames[j];

                if (metricsByName.TryGetValue(metric1, out var values1) &&
                    metricsByName.TryGetValue(metric2, out var values2))
                {
                    // Find common time points
                    var commonTimes = values1.Keys.Intersect(values2.Keys).ToList();
                    
                    if (commonTimes.Count >= 2)
                    {
                        var x = commonTimes.Select(t => values1[t]).ToArray();
                        var y = commonTimes.Select(t => values2[t]).ToArray();

                        var correlation = CalculatePearsonCorrelation(x, y);
                        var strength = GetCorrelationStrength(Math.Abs(correlation));

                        results.Add(new CorrelationResultDto(
                            metric1,
                            metric2,
                            (decimal)correlation,
                            strength
                        ));
                    }
                }
            }
        }

        return results;
    }

    private static double CalculatePearsonCorrelation(double[] x, double[] y)
    {
        if (x.Length != y.Length || x.Length == 0)
            return 0;

        var meanX = x.Average();
        var meanY = y.Average();

        var numerator = x.Zip(y, (xi, yi) => (xi - meanX) * (yi - meanY)).Sum();
        var denominatorX = Math.Sqrt(x.Select(xi => Math.Pow(xi - meanX, 2)).Sum());
        var denominatorY = Math.Sqrt(y.Select(yi => Math.Pow(yi - meanY, 2)).Sum());

        if (denominatorX == 0 || denominatorY == 0)
            return 0;

        return numerator / (denominatorX * denominatorY);
    }

    private static string GetCorrelationStrength(double absoluteCorrelation)
    {
        return absoluteCorrelation switch
        {
            >= 0.8 => "Very Strong",
            >= 0.6 => "Strong",
            >= 0.4 => "Moderate",
            >= 0.2 => "Weak",
            _ => "Very Weak"
        };
    }
}