namespace ApiService.Metrics.Commands;

using MediatR;
using Data;
using Restaurant.DTO;

public record BulkImportMetricsCommand(BulkMetricImportDto ImportData) : IRequest<int>;

public class BulkImportMetricsHandler(RestaurantDbContext context) : IRequestHandler<BulkImportMetricsCommand, int>
{
    public async Task<int> Handle(BulkImportMetricsCommand request, CancellationToken cancellationToken)
    {
        var dto = request.ImportData;
        var entities = new List<MetricValue>();

        foreach (var metric in dto.Metrics)
        {
            entities.Add(new MetricValue
            {
                RestaurantId = dto.RestaurantId,
                MetricName = metric.MetricName,
                Value = metric.Value,
                Timestamp = metric.Timestamp,
                Unit = metric.Unit,
                Tags = metric.Tags,
                CreatedAt = DateTime.UtcNow
            });
        }

        context.MetricValues.AddRange(entities);
        await context.SaveChangesAsync(cancellationToken);

        return entities.Count;
    }
}