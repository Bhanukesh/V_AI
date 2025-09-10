namespace Data;

public class RestaurantDbContext(DbContextOptions<RestaurantDbContext> options) : DbContext(options)
{
    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<Restaurant> Restaurants => Set<Restaurant>();
    public DbSet<MetricValue> MetricValues => Set<MetricValue>();
    public DbSet<Revenue> Revenues => Set<Revenue>();
    public DbSet<Recommendation> Recommendations => Set<Recommendation>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Organization configuration
        modelBuilder.Entity<Organization>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(1000);
            entity.HasIndex(e => e.Name);
        });

        // Restaurant configuration
        modelBuilder.Entity<Restaurant>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Address).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.TimeZoneId).IsRequired().HasMaxLength(50);

            entity.HasOne(e => e.Organization)
                  .WithMany(e => e.Restaurants)
                  .HasForeignKey(e => e.OrganizationId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.OrganizationId);
            entity.HasIndex(e => e.Name);
        });

        // MetricValue configuration
        modelBuilder.Entity<MetricValue>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.MetricName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Value).HasPrecision(18, 4);
            entity.Property(e => e.Unit).HasMaxLength(20);
            entity.Property(e => e.Tags).HasMaxLength(2000);

            entity.HasOne(e => e.Restaurant)
                  .WithMany(e => e.MetricValues)
                  .HasForeignKey(e => e.RestaurantId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Time-series optimized indexes
            entity.HasIndex(e => new { e.RestaurantId, e.MetricName, e.Timestamp })
                  .HasDatabaseName("IX_MetricValues_TimeSeries");
            entity.HasIndex(e => e.Timestamp)
                  .HasDatabaseName("IX_MetricValues_Timestamp");
        });

        // Revenue configuration
        modelBuilder.Entity<Revenue>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TotalRevenue).HasPrecision(18, 2);
            entity.Property(e => e.FoodRevenue).HasPrecision(18, 2);
            entity.Property(e => e.BeverageRevenue).HasPrecision(18, 2);
            entity.Property(e => e.AverageTicket).HasPrecision(18, 2);
            entity.Property(e => e.Notes).HasMaxLength(1000);

            entity.HasOne(e => e.Restaurant)
                  .WithMany(e => e.Revenues)
                  .HasForeignKey(e => e.RestaurantId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Unique constraint on restaurant and date
            entity.HasIndex(e => new { e.RestaurantId, e.Date })
                  .IsUnique()
                  .HasDatabaseName("IX_Revenue_RestaurantDate");
        });

        // Recommendation configuration
        modelBuilder.Entity<Recommendation>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.Type).HasConversion<string>().HasMaxLength(20);
            entity.Property(e => e.Priority).HasConversion<string>().HasMaxLength(20);
            entity.Property(e => e.EstimatedImpact).HasPrecision(18, 2);
            entity.Property(e => e.ActionItems).HasMaxLength(4000);

            entity.HasOne(e => e.Restaurant)
                  .WithMany(e => e.Recommendations)
                  .HasForeignKey(e => e.RestaurantId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.RestaurantId);
            entity.HasIndex(e => new { e.Type, e.Priority });
            entity.HasIndex(e => e.GeneratedAt);
        });
    }
}
