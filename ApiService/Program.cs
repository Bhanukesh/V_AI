using System.Reflection;
using ApiService.Python;
using ApiService.Services;
using Data;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});
builder.Services.AddProblemDetails();
builder.Services.AddCors();
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
});
builder.Services.AddEndpointsApiExplorer();

// Add database context
builder.AddSqlServerDbContext<RestaurantDbContext>("restaurantdb");

builder.Services.AddOpenApiDocument(options =>
{
    options.DocumentName = "v1";
    options.Title = "Vida AI Restaurant Analytics API";
    options.Version = "v1";
    options.UseHttpAttributeNameAsOperationId = true;

    options.PostProcess = document =>
    {
        document.BasePath = "/";
    };
});

builder.Services.AddHttpClient<PythonClient>(
    static client => client.BaseAddress = new("http://pythonapi"));

// Add Python analytics service with HttpClient
builder.Services.AddHttpClient<IPythonAnalyticsService, PythonAnalyticsService>(
    client => 
    {
        client.BaseAddress = new Uri("http://pythonapi");
        client.Timeout = TimeSpan.FromSeconds(30);
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();
app.UseCors(static builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader()
           .WithExposedHeaders("*");
});
app.MapDefaultEndpoints();
app.MapControllers();
app.UseOpenApi();
app.UseSwaggerUi();
app.Run();