using Microsoft.EntityFrameworkCore;
using Data;
using MigrationService;

// Check for command line arguments
if (args.Length > 0 && args[0].Equals("seed", StringComparison.OrdinalIgnoreCase))
{
    await RunSeedOnlyAsync(args);
    return;
}

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

builder.Services.AddHostedService<Worker>();
builder.Services.AddScoped<RestaurantDataSeeder>();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

// Add database context
builder.AddSqlServerDbContext<RestaurantDbContext>("restaurantdb", null,
    optionsBuilder => optionsBuilder.UseSqlServer(options => 
    options.MigrationsAssembly(typeof(Program).Assembly.GetName().Name)));

var app = builder.Build();

app.MapDefaultEndpoints();
app.Run();

// Standalone seeding method
static async Task RunSeedOnlyAsync(string[] args)
{
    Console.WriteLine("🌱 Restaurant Demo Data Seeder");
    Console.WriteLine("==============================");
    
    var builder = WebApplication.CreateBuilder(args);
    
    // Configure logging for console output
    builder.Logging.ClearProviders();
    builder.Logging.AddConsole();
    
    // Add database context with connection string
    var connectionString = builder.Configuration.GetConnectionString("restaurantdb") 
        ?? "Data Source=localhost,9000;Initial Catalog=restaurantdb;User ID=sa;Password=yourStrong(!)Password;TrustServerCertificate=true";
    
    builder.Services.AddDbContext<RestaurantDbContext>(options =>
        options.UseSqlServer(connectionString, sqlOptions =>
            sqlOptions.MigrationsAssembly(typeof(Program).Assembly.GetName().Name)));
    
    builder.Services.AddScoped<RestaurantDataSeeder>();
    
    var app = builder.Build();
    
    try
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<RestaurantDbContext>();
        var seeder = scope.ServiceProvider.GetRequiredService<RestaurantDataSeeder>();
        
        Console.WriteLine("🔗 Testing database connection...");
        await dbContext.Database.EnsureCreatedAsync();
        
        Console.WriteLine("📊 Starting data seeding process...");
        await seeder.SeedAsync();
        
        Console.WriteLine("✅ Demo data seeding completed successfully!");
        Console.WriteLine();
        
        // Display summary
        var orgCount = await dbContext.Organizations.CountAsync();
        var restaurantCount = await dbContext.Restaurants.CountAsync();
        var revenueCount = await dbContext.Revenues.CountAsync();
        var metricCount = await dbContext.MetricValues.CountAsync();
        var recommendationCount = await dbContext.Recommendations.CountAsync();
        
        Console.WriteLine("📈 Data Summary:");
        Console.WriteLine($"   Organizations: {orgCount}");
        Console.WriteLine($"   Restaurants: {restaurantCount}");
        Console.WriteLine($"   Revenue Records: {revenueCount:N0}");
        Console.WriteLine($"   Metric Values: {metricCount:N0}");
        Console.WriteLine($"   Recommendations: {recommendationCount}");
        Console.WriteLine();
        Console.WriteLine("🎯 Ready to test your restaurant performance dashboard!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error during seeding: {ex.Message}");
        if (ex.InnerException != null)
        {
            Console.WriteLine($"   Inner exception: {ex.InnerException.Message}");
        }
        Environment.Exit(1);
    }
}

