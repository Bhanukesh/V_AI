namespace ApiService.Restaurant.Queries;

using MediatR;
using Data;
using Restaurant.DTO;
using Microsoft.EntityFrameworkCore;

public record GetRestaurantKpisQuery(int RestaurantId, string Period = "last_30d") : IRequest<RestaurantKpiDto?>;

public class GetRestaurantKpisHandler(RestaurantDbContext context) : IRequestHandler<GetRestaurantKpisQuery, RestaurantKpiDto?>
{
    public async Task<RestaurantKpiDto?> Handle(GetRestaurantKpisQuery request, CancellationToken cancellationToken)
    {
        var restaurant = await context.Restaurants.FirstOrDefaultAsync(r => r.Id == request.RestaurantId, cancellationToken);
        
        if (restaurant == null)
        {
            return null;
        }

        var (startDate, endDate) = GetPeriodDates(request.Period);

        // Get revenue data
        var revenueData = await context.Revenues
            .Where(r => r.RestaurantId == request.RestaurantId && 
                       r.Date >= DateOnly.FromDateTime(startDate) && 
                       r.Date <= DateOnly.FromDateTime(endDate))
            .ToListAsync(cancellationToken);

        var totalRevenue = revenueData.Sum(r => r.TotalRevenue);
        var averageRevenue = revenueData.Any() ? revenueData.Average(r => r.TotalRevenue) : 0;
        var totalTransactions = revenueData.Sum(r => r.TransactionCount);

        // Get metric data
        var metricData = await context.MetricValues
            .Where(m => m.RestaurantId == request.RestaurantId &&
                       m.Timestamp >= startDate &&
                       m.Timestamp <= endDate)
            .ToListAsync(cancellationToken);

        var prepTimeAvg = metricData.Where(m => m.MetricName == "prep_time").Average(m => (decimal?)m.Value) ?? 0;
        var tableTurnoverAvg = metricData.Where(m => m.MetricName == "table_turnover").Average(m => (decimal?)m.Value) ?? 0;
        var orderAccuracyAvg = metricData.Where(m => m.MetricName == "order_accuracy").Average(m => (decimal?)m.Value) ?? 0;
        var customerSatisfactionAvg = metricData.Where(m => m.MetricName == "customer_satisfaction").Average(m => (decimal?)m.Value) ?? 0;

        return new RestaurantKpiDto(
            restaurant.Id,
            restaurant.Name,
            totalRevenue,
            averageRevenue,
            prepTimeAvg,
            tableTurnoverAvg,
            orderAccuracyAvg,
            customerSatisfactionAvg,
            totalTransactions,
            startDate,
            endDate
        );
    }

    private static (DateTime StartDate, DateTime EndDate) GetPeriodDates(string period)
    {
        var endDate = DateTime.UtcNow;
        var startDate = period switch
        {
            "last_7d" => endDate.AddDays(-7),
            "last_30d" => endDate.AddDays(-30),
            "last_90d" => endDate.AddDays(-90),
            "last_year" => endDate.AddYears(-1),
            _ => endDate.AddDays(-30)
        };

        return (startDate, endDate);
    }
}