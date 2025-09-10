namespace ApiService.Restaurant.DTO;

public record RestaurantDto(
    int Id,
    string Name,
    string Address,
    string? Phone,
    string? Email,
    string TimeZoneId,
    DateTime CreatedAt,
    DateTime UpdatedAt,
    int OrganizationId
);

public record CreateRestaurantDto(
    string Name,
    string Address,
    string? Phone,
    string? Email,
    string TimeZoneId,
    int OrganizationId
);

public record UpdateRestaurantDto(
    string Name,
    string Address,
    string? Phone,
    string? Email,
    string TimeZoneId
);

public record RestaurantKpiDto(
    int RestaurantId,
    string RestaurantName,
    decimal TotalRevenue,
    decimal AverageRevenue,
    decimal PrepTimeAverage,
    decimal TableTurnoverAverage,
    decimal OrderAccuracyAverage,
    decimal CustomerSatisfactionAverage,
    int TotalTransactions,
    DateTime PeriodStart,
    DateTime PeriodEnd
);

public record MetricValueDto(
    long Id,
    string MetricName,
    decimal Value,
    DateTime Timestamp,
    string? Unit,
    string? Tags
);

public record CreateMetricValueDto(
    string MetricName,
    decimal Value,
    DateTime Timestamp,
    string? Unit,
    string? Tags
);

public record BulkMetricImportDto(
    int RestaurantId,
    CreateMetricValueDto[] Metrics
);

public record CorrelationRequestDto(
    int RestaurantId,
    string[] MetricNames,
    DateTime StartDate,
    DateTime EndDate
);

public record CorrelationResultDto(
    string Metric1,
    string Metric2,
    decimal CorrelationCoefficient,
    string Strength
);