# Restaurant Demo Data Seeder 🌱

This tool generates realistic demo data for the restaurant performance management system.

## Features

### Generated Data
- **5 Demo Restaurants** with different characteristics:
  - The Golden Fork (High-end, dinner only)
  - Bella Vista Italian (Mid-high range, lunch & dinner)
  - Harbor Grill (Upscale seafood, seasonal patterns)
  - Spice Route (Ethnic cuisine, high variability)
  - The Corner Bistro (Neighborhood spot, all-day)

### Data Volume
- **1 Year** of revenue data (365 records per restaurant = 1,825 total)
- **Hourly metrics** for operational data (~50,000+ metric values)
- **Smart correlations** between metrics and revenue
- **Realistic patterns**: weekends, seasons, rush hours

### Metrics Generated
- `prep_time`: 8-15 minutes (kitchen efficiency)
- `table_turnover`: 45-105 minutes (dining duration)
- `order_accuracy`: 85-99% (order correctness)
- `customer_satisfaction`: 2.0-5.0 rating (customer experience)
- `wait_time`: 5-25 minutes (service speed)

### Realistic Patterns
- **Weekend Boost**: 10-40% higher revenue Fri/Sat
- **Seasonal Trends**: December +30%, Jan/Feb -30%
- **Rush Hour Impact**: Higher prep times, lower satisfaction during 12-2pm and 6-8pm
- **Performance Correlation**: Better restaurants have faster service and higher satisfaction

## Usage

### Command Line Seeding
```bash
# Generate full demo data (recommended)
dotnet run --project MigrationService seed

# Expected output:
🌱 Restaurant Demo Data Seeder
==============================
🔗 Testing database connection...
📊 Starting data seeding process...
✅ Demo data seeding completed successfully!

📈 Data Summary:
   Organizations: 1
   Restaurants: 5
   Revenue Records: 1,825
   Metric Values: 50,000+
   Recommendations: 15-20
```

### Integration with Aspire
The seeder integrates with your existing Aspire orchestration:

```bash
# Start the full application (includes auto-migration + basic seeding)
dotnet run --project AppHost

# Then run additional demo data
dotnet run --project MigrationService seed
```

## Data Characteristics

### Restaurant Performance Levels
| Restaurant | Base Revenue | Variability | Weekend Boost | Seasonal Pattern |
|------------|-------------|------------|---------------|------------------|
| The Golden Fork | $3,500/day | 30% | 40% | Standard |
| Bella Vista Italian | $2,800/day | 25% | 30% | +10% seasonal |
| Harbor Grill | $3,200/day | 20% | 20% | +20% seasonal |
| Spice Route | $2,200/day | 40% | 50% | -10% seasonal |
| The Corner Bistro | $1,800/day | 15% | 10% | -20% seasonal |

### Metric Correlations
- **High performers** (Golden Fork, Harbor Grill):
  - Faster prep times (10-12 min avg)
  - Higher customer satisfaction (4.2-4.7)
  - Better order accuracy (92-96%)

- **Variable performers** (Spice Route):
  - More variable prep times (8-18 min range)
  - Customer satisfaction varies with service quality
  - Higher weekend impact on metrics

### Time Patterns
- **Operating Hours**: Realistic per restaurant type
- **Rush Hours**: 12-2pm (lunch), 6-8pm (dinner)
- **Closed Days**: Some restaurants closed Mondays (10% chance)
- **Holiday Patterns**: December boost, January/February slowdown

## Benefits for Demo/Testing

1. **Realistic Dashboard Data**: KPIs show meaningful ranges and correlations
2. **Analytics Ready**: Data supports correlation analysis and forecasting
3. **Performance Comparisons**: Different restaurant types for comparison
4. **Time Series**: Full year of data for trend analysis
5. **Actionable Insights**: Generated recommendations based on actual performance patterns

## Technical Details

### Data Generation Algorithm
- Fixed seed (42) for consistent demo data
- Performance-based metric generation
- Realistic random variations
- Time-based stress factors (rush hours, weekends)
- Seasonal and operational pattern modeling

### Database Optimization
- Time-series optimized indexes
- Bulk insert operations
- Smart batching for large datasets
- Foreign key relationships maintained

This seeding tool provides production-ready demo data that showcases the full capabilities of your restaurant performance management system.