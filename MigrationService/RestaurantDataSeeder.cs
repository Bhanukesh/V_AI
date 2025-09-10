using Data;
using Microsoft.EntityFrameworkCore;

namespace MigrationService;

public class RestaurantDataSeeder(RestaurantDbContext dbContext, ILogger<RestaurantDataSeeder> logger)
{
    private readonly Random _random = new(42); // Fixed seed for consistent demo data

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Starting restaurant data seeding...");

        // Check if data already exists
        if (await dbContext.Organizations.AnyAsync(cancellationToken))
        {
            logger.LogInformation("Demo data already exists, skipping seeding.");
            return;
        }

        await SeedOrganizationsAsync(cancellationToken);
        await SeedRestaurantsAsync(cancellationToken);
        await SeedRevenueDataAsync(cancellationToken);
        await SeedMetricDataAsync(cancellationToken);
        await SeedRecommendationsAsync(cancellationToken);

        logger.LogInformation("Restaurant data seeding completed successfully.");
    }

    private async Task SeedOrganizationsAsync(CancellationToken cancellationToken)
    {
        var organizations = new[]
        {
            new Organization
            {
                Name = "Demo Restaurant Group",
                Description = "A premier restaurant group specializing in fine dining experiences",
                CreatedAt = DateTime.UtcNow.AddMonths(-12),
                UpdatedAt = DateTime.UtcNow
            }
        };

        dbContext.Organizations.AddRange(organizations);
        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Seeded {Count} organizations", organizations.Length);
    }

    private async Task SeedRestaurantsAsync(CancellationToken cancellationToken)
    {
        var organization = await dbContext.Organizations.FirstAsync(cancellationToken);

        var restaurants = new[]
        {
            new Restaurant
            {
                Name = "The Golden Fork",
                Address = "123 Main Street, Downtown, NY 10001",
                Phone = "+1-555-0123",
                Email = "info@goldenfork.demo",
                TimeZoneId = "America/New_York",
                OrganizationId = organization.Id,
                CreatedAt = DateTime.UtcNow.AddMonths(-12),
                UpdatedAt = DateTime.UtcNow
            },
            new Restaurant
            {
                Name = "Bella Vista Italian",
                Address = "456 Oak Avenue, Midtown, NY 10002",
                Phone = "+1-555-0456",
                Email = "contact@bellavista.demo",
                TimeZoneId = "America/New_York",
                OrganizationId = organization.Id,
                CreatedAt = DateTime.UtcNow.AddMonths(-10),
                UpdatedAt = DateTime.UtcNow
            },
            new Restaurant
            {
                Name = "Harbor Grill",
                Address = "789 Waterfront Drive, Waterfront, NY 10003",
                Phone = "+1-555-0789",
                Email = "reservations@harborgrill.demo",
                TimeZoneId = "America/New_York",
                OrganizationId = organization.Id,
                CreatedAt = DateTime.UtcNow.AddMonths(-8),
                UpdatedAt = DateTime.UtcNow
            },
            new Restaurant
            {
                Name = "Spice Route",
                Address = "321 Cultural Boulevard, Arts District, NY 10004",
                Phone = "+1-555-0321",
                Email = "info@spiceroute.demo",
                TimeZoneId = "America/New_York",
                OrganizationId = organization.Id,
                CreatedAt = DateTime.UtcNow.AddMonths(-6),
                UpdatedAt = DateTime.UtcNow
            },
            new Restaurant
            {
                Name = "The Corner Bistro",
                Address = "654 Elm Street, Residential, NY 10005",
                Phone = "+1-555-0654",
                Email = "hello@cornerbistro.demo",
                TimeZoneId = "America/New_York",
                OrganizationId = organization.Id,
                CreatedAt = DateTime.UtcNow.AddMonths(-4),
                UpdatedAt = DateTime.UtcNow
            }
        };

        dbContext.Restaurants.AddRange(restaurants);
        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Seeded {Count} restaurants", restaurants.Length);
    }

    private async Task SeedRevenueDataAsync(CancellationToken cancellationToken)
    {
        var restaurants = await dbContext.Restaurants.ToListAsync(cancellationToken);
        var startDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-1));
        var endDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));

        foreach (var restaurant in restaurants)
        {
            await SeedRestaurantRevenueAsync(restaurant, startDate, endDate, cancellationToken);
        }

        logger.LogInformation("Seeded revenue data for {Count} restaurants over {Days} days", 
            restaurants.Count, endDate.DayNumber - startDate.DayNumber + 1);
    }

    private async Task SeedRestaurantRevenueAsync(Restaurant restaurant, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken)
    {
        // Restaurant-specific performance characteristics
        var (baseRevenue, variability, weekendMultiplier, seasonalPattern) = GetRestaurantCharacteristics(restaurant.Name);

        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            var dayOfWeek = date.DayOfWeek;
            var monthOfYear = date.Month;
            
            // Weekend boost
            var weekendBoost = dayOfWeek == DayOfWeek.Friday || dayOfWeek == DayOfWeek.Saturday ? weekendMultiplier : 1.0;
            
            // Seasonal pattern (higher in Dec, lower in Jan-Feb)
            var seasonalBoost = monthOfYear switch
            {
                12 => seasonalPattern * 1.3, // December holiday boost
                1 or 2 => seasonalPattern * 0.7, // January/February slowdown
                11 => seasonalPattern * 1.15, // November pre-holiday
                6 or 7 or 8 => seasonalPattern * 1.1, // Summer boost
                _ => seasonalPattern
            };

            // Random daily variation
            var randomVariation = 0.8 + (_random.NextDouble() * 0.4); // ±20% variation

            var totalRevenue = (decimal)(baseRevenue * weekendBoost * seasonalBoost * randomVariation);
            var foodPercentage = 0.65 + (_random.NextDouble() * 0.15); // 65-80% food
            var foodRevenue = totalRevenue * (decimal)foodPercentage;
            var beverageRevenue = totalRevenue - foodRevenue;

            // Transaction count correlated with revenue
            var avgTicket = 35 + (_random.NextDouble() * 25); // $35-60 average
            var transactionCount = (int)(totalRevenue / (decimal)avgTicket);

            var revenue = new Revenue
            {
                Date = date,
                TotalRevenue = Math.Round(totalRevenue, 2),
                FoodRevenue = Math.Round(foodRevenue, 2),
                BeverageRevenue = Math.Round(beverageRevenue, 2),
                TransactionCount = transactionCount,
                AverageTicket = Math.Round(totalRevenue / transactionCount, 2),
                RestaurantId = restaurant.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            dbContext.Revenues.Add(revenue);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedMetricDataAsync(CancellationToken cancellationToken)
    {
        var restaurants = await dbContext.Restaurants.ToListAsync(cancellationToken);
        var startDate = DateTime.Today.AddYears(-1);
        var endDate = DateTime.Today.AddDays(-1);

        foreach (var restaurant in restaurants)
        {
            await SeedRestaurantMetricsAsync(restaurant, startDate, endDate, cancellationToken);
        }

        logger.LogInformation("Seeded metric data for {Count} restaurants", restaurants.Count);
    }

    private async Task SeedRestaurantMetricsAsync(Restaurant restaurant, DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
    {
        var (baseRevenue, _, _, _) = GetRestaurantCharacteristics(restaurant.Name);
        var performanceLevel = baseRevenue / 3000.0; // Normalize to 0.5-1.5 range

        var metrics = new[] { "prep_time", "table_turnover", "order_accuracy", "customer_satisfaction", "wait_time" };

        for (var date = startDate; date <= endDate; date = date.AddDays(1))
        {
            // Skip metrics for days when restaurant might be closed (some Mondays)
            if (date.DayOfWeek == DayOfWeek.Monday && _random.NextDouble() < 0.1)
                continue;

            // Generate hourly metrics during operating hours
            var operatingHours = GetOperatingHours(restaurant.Name);
            
            for (int hour = operatingHours.start; hour <= operatingHours.end; hour++)
            {
                var timestamp = date.Date.AddHours(hour);
                
                foreach (var metric in metrics)
                {
                    var value = GenerateCorrelatedMetricValue(metric, performanceLevel, hour, date.DayOfWeek);
                    var unit = GetMetricUnit(metric);

                    var metricValue = new MetricValue
                    {
                        MetricName = metric,
                        Value = Math.Round(value, 2),
                        Timestamp = timestamp,
                        Unit = unit,
                        RestaurantId = restaurant.Id,
                        CreatedAt = DateTime.UtcNow
                    };

                    dbContext.MetricValues.Add(metricValue);
                }
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private decimal GenerateCorrelatedMetricValue(string metricName, double performanceLevel, int hour, DayOfWeek dayOfWeek)
    {
        // Base values influenced by performance level (better performing restaurants have better metrics)
        var rushHour = hour >= 12 && hour <= 14 || hour >= 18 && hour <= 20; // Lunch and dinner rush
        var weekendBusy = dayOfWeek == DayOfWeek.Friday || dayOfWeek == DayOfWeek.Saturday;
        
        var rushMultiplier = rushHour ? 1.2 : 1.0;
        var weekendMultiplier = weekendBusy ? 1.1 : 1.0;
        var stressLevel = rushMultiplier * weekendMultiplier;

        return metricName switch
        {
            "prep_time" => GeneratePrepTime(performanceLevel, stressLevel),
            "table_turnover" => GenerateTableTurnover(performanceLevel, stressLevel),
            "order_accuracy" => GenerateOrderAccuracy(performanceLevel, stressLevel),
            "customer_satisfaction" => GenerateCustomerSatisfaction(performanceLevel, stressLevel),
            "wait_time" => GenerateWaitTime(performanceLevel, stressLevel),
            _ => (decimal)(_random.NextDouble() * 100)
        };
    }

    private decimal GeneratePrepTime(double performanceLevel, double stressLevel)
    {
        // Better restaurants have faster prep times, but stress increases it
        var baseTime = 15 - (performanceLevel * 5); // 10-15 minutes base
        var stressImpact = stressLevel * 3; // Up to +3.6 minutes during rush
        var randomVariation = (_random.NextDouble() - 0.5) * 4; // ±2 minutes random
        
        return (decimal)Math.Max(8, baseTime + stressImpact + randomVariation);
    }

    private decimal GenerateTableTurnover(double performanceLevel, double stressLevel)
    {
        // Time per table (lower is better for turnover rate)
        var baseTime = 90 - (performanceLevel * 30); // 60-90 minutes base
        var stressImpact = stressLevel * 15; // Up to +18 minutes during rush
        var randomVariation = (_random.NextDouble() - 0.5) * 20; // ±10 minutes random
        
        return (decimal)Math.Max(45, baseTime + stressImpact + randomVariation);
    }

    private decimal GenerateOrderAccuracy(double performanceLevel, double stressLevel)
    {
        // Better restaurants have higher accuracy, stress reduces it
        var baseAccuracy = 0.85 + (performanceLevel * 0.10); // 85-95% base
        var stressImpact = -(stressLevel - 1) * 0.05; // Up to -6% during rush
        var randomVariation = (_random.NextDouble() - 0.5) * 0.08; // ±4% random
        
        return (decimal)Math.Min(0.99, Math.Max(0.75, baseAccuracy + stressImpact + randomVariation));
    }

    private decimal GenerateCustomerSatisfaction(double performanceLevel, double stressLevel)
    {
        // Better restaurants have higher satisfaction, correlated with other metrics
        var baseRating = 3.5 + (performanceLevel * 1.2); // 3.5-4.7 base
        var stressImpact = -(stressLevel - 1) * 0.3; // Up to -0.36 during rush
        var randomVariation = (_random.NextDouble() - 0.5) * 0.4; // ±0.2 random
        
        return (decimal)Math.Min(5.0, Math.Max(2.0, baseRating + stressImpact + randomVariation));
    }

    private decimal GenerateWaitTime(double performanceLevel, double stressLevel)
    {
        // Better restaurants have lower wait times, stress increases it
        var baseWait = 20 - (performanceLevel * 8); // 12-20 minutes base
        var stressImpact = stressLevel * 8; // Up to +9.6 minutes during rush
        var randomVariation = (_random.NextDouble() - 0.5) * 10; // ±5 minutes random
        
        return (decimal)Math.Max(5, baseWait + stressImpact + randomVariation);
    }

    private async Task SeedRecommendationsAsync(CancellationToken cancellationToken)
    {
        var restaurants = await dbContext.Restaurants.ToListAsync(cancellationToken);
        var recommendationTemplates = GetRecommendationTemplates();

        foreach (var restaurant in restaurants)
        {
            // Generate 2-4 recommendations per restaurant
            var numRecommendations = _random.Next(2, 5);
            var selectedTemplates = recommendationTemplates.OrderBy(x => _random.Next()).Take(numRecommendations);

            foreach (var template in selectedTemplates)
            {
                var recommendation = new Recommendation
                {
                    Title = template.Title,
                    Description = template.Description,
                    Type = template.Type,
                    Priority = template.Priority,
                    EstimatedImpact = (decimal)(_random.NextDouble() * 20 + 5), // 5-25% impact
                    ActionItems = template.ActionItems,
                    RestaurantId = restaurant.Id,
                    GeneratedAt = DateTime.UtcNow.AddDays(-_random.Next(0, 30)),
                    ExpiresAt = DateTime.UtcNow.AddDays(_random.Next(30, 90))
                };

                dbContext.Recommendations.Add(recommendation);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Seeded recommendations for {Count} restaurants", restaurants.Count);
    }

    private static (double baseRevenue, double variability, double weekendMultiplier, double seasonalPattern) GetRestaurantCharacteristics(string restaurantName)
    {
        return restaurantName switch
        {
            "The Golden Fork" => (3500, 0.3, 1.4, 1.0), // High-end, high variability, big weekend boost
            "Bella Vista Italian" => (2800, 0.25, 1.3, 1.1), // Mid-high range, steady, good weekends
            "Harbor Grill" => (3200, 0.2, 1.2, 1.2), // Upscale seafood, seasonal tourist boost
            "Spice Route" => (2200, 0.4, 1.5, 0.9), // Ethnic cuisine, high variability, weekend heavy
            "The Corner Bistro" => (1800, 0.15, 1.1, 0.8), // Neighborhood spot, consistent, lower volume
            _ => (2500, 0.25, 1.25, 1.0) // Default characteristics
        };
    }

    private static (int start, int end) GetOperatingHours(string restaurantName)
    {
        return restaurantName switch
        {
            "The Golden Fork" => (17, 22), // Dinner only (5 PM - 10 PM)
            "Bella Vista Italian" => (11, 22), // Lunch and dinner (11 AM - 10 PM)
            "Harbor Grill" => (16, 23), // Late afternoon to late night (4 PM - 11 PM)
            "Spice Route" => (11, 21), // Lunch and early dinner (11 AM - 9 PM)
            "The Corner Bistro" => (8, 20), // All day (8 AM - 8 PM)
            _ => (11, 21) // Default hours
        };
    }

    private static string GetMetricUnit(string metricName)
    {
        return metricName switch
        {
            "prep_time" => "minutes",
            "table_turnover" => "minutes",
            "order_accuracy" => "percentage",
            "customer_satisfaction" => "rating",
            "wait_time" => "minutes",
            _ => "units"
        };
    }

    private static List<(string Title, string Description, RecommendationType Type, RecommendationPriority Priority, string ActionItems)> GetRecommendationTemplates()
    {
        return new List<(string, string, RecommendationType, RecommendationPriority, string)>
        {
            ("Optimize Kitchen Prep Time", "Average prep time has increased beyond optimal range. Consider staff retraining and workflow optimization.", RecommendationType.Operations, RecommendationPriority.High, "[\"Reorganize prep stations\",\"Train staff on mise en place\",\"Review prep time standards\"]"),
            ("Improve Order Accuracy", "Order accuracy has declined below 90%. Focus on kitchen communication and order verification processes.", RecommendationType.Performance, RecommendationPriority.High, "[\"Implement order verification system\",\"Train kitchen staff\",\"Review order taking process\"]"),
            ("Reduce Customer Wait Time", "Average wait time exceeds customer expectations. Consider staffing adjustments during peak hours.", RecommendationType.Staffing, RecommendationPriority.Medium, "[\"Analyze peak hour staffing\",\"Implement reservation system\",\"Train hosts on wait time management\"]"),
            ("Increase Beverage Revenue", "Beverage sales below industry average. Focus on staff training and menu promotion strategies.", RecommendationType.Revenue, RecommendationPriority.Medium, "[\"Create cocktail pairing menu\",\"Train servers on upselling\",\"Implement beverage promotions\"]"),
            ("Enhance Customer Satisfaction", "Customer satisfaction scores show room for improvement. Focus on service training and experience enhancement.", RecommendationType.Performance, RecommendationPriority.Medium, "[\"Implement service training program\",\"Review customer feedback\",\"Enhance dining atmosphere\"]"),
            ("Optimize Table Turnover", "Table turnover rate could be improved to increase revenue potential during peak hours.", RecommendationType.Operations, RecommendationPriority.Low, "[\"Analyze seating patterns\",\"Optimize service timing\",\"Consider reservation policies\"]"),
            ("Menu Engineering Analysis", "Analyze menu item profitability and popularity to optimize offerings and increase margins.", RecommendationType.Revenue, RecommendationPriority.Low, "[\"Analyze menu item performance\",\"Review food costs\",\"Consider menu restructuring\"]"),
            ("Staff Scheduling Optimization", "Labor costs could be optimized through better shift scheduling aligned with customer traffic patterns.", RecommendationType.Operations, RecommendationPriority.Medium, "[\"Analyze traffic patterns\",\"Optimize shift schedules\",\"Cross-train staff for flexibility\"]")
        };
    }
}