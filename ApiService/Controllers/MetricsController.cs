namespace ApiService.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Metrics.Commands;
using Metrics.Queries;
using Restaurant.DTO;

[ApiController]
[Route("api/restaurants/{restaurantId}/[controller]")]
public class MetricsController(IMediator mediator) : ControllerBase
{
    [HttpGet(Name = nameof(GetRestaurantMetrics))]
    public async Task<ActionResult<IEnumerable<MetricValueDto>>> GetRestaurantMetrics(
        int restaurantId,
        [FromQuery] string? metricName = null,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 100)
    {
        try
        {
            var metrics = await mediator.Send(new GetRestaurantMetricsQuery(
                restaurantId, metricName, startDate, endDate, page, pageSize));
            
            return Ok(metrics);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("bulk", Name = nameof(BulkImportMetrics))]
    public async Task<ActionResult<int>> BulkImportMetrics(int restaurantId, CreateMetricValueDto[] metrics)
    {
        try
        {
            var importData = new BulkMetricImportDto(restaurantId, metrics);
            var count = await mediator.Send(new BulkImportMetricsCommand(importData));
            
            return Ok(new { ImportedCount = count });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}