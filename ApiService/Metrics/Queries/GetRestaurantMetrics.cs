namespace ApiService.Metrics.Queries;

using MediatR;
using Data;
using Restaurant.DTO;
using Microsoft.EntityFrameworkCore;

public record GetRestaurantMetricsQuery(
    int RestaurantId,
    string? MetricName = null,
    DateTime? StartDate = null,
    DateTime? EndDate = null,
    int Page = 1,
    int PageSize = 100
) : IRequest<IEnumerable<MetricValueDto>>;

public class GetRestaurantMetricsHandler(RestaurantDbContext context) : IRequestHandler<GetRestaurantMetricsQuery, IEnumerable<MetricValueDto>>
{
    public async Task<IEnumerable<MetricValueDto>> Handle(GetRestaurantMetricsQuery request, CancellationToken cancellationToken)
    {
        var query = context.MetricValues
            .Where(m => m.RestaurantId == request.RestaurantId);

        if (!string.IsNullOrEmpty(request.MetricName))
        {
            query = query.Where(m => m.MetricName == request.MetricName);
        }

        if (request.StartDate.HasValue)
        {
            query = query.Where(m => m.Timestamp >= request.StartDate.Value);
        }

        if (request.EndDate.HasValue)
        {
            query = query.Where(m => m.Timestamp <= request.EndDate.Value);
        }

        return await query
            .OrderByDescending(m => m.Timestamp)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(m => new MetricValueDto(
                m.Id,
                m.MetricName,
                m.Value,
                m.Timestamp,
                m.Unit,
                m.Tags
            ))
            .ToListAsync(cancellationToken);
    }
}