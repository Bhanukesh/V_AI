var builder = DistributedApplication.CreateBuilder(args);

var sqlPassword = builder.AddParameter("sql-password", secret: true);

var sqlServer = builder
        .AddSqlServer("restaurant-sqlserver", password: sqlPassword, port: 9000)
        .WithLifetime(ContainerLifetime.Persistent)
        .AddDatabase("restaurantdb");

var migrationService = builder.AddProject<Projects.MigrationService>("migrationservice")
    .WithReference(sqlServer)
    .WaitFor(sqlServer);

// The Python API is experimental and subject to change
#pragma warning disable ASPIREHOSTINGPYTHON001
var pythonApi = builder.AddPythonApp("pythonapi","../PythonApi","run_app.py")
    .WithHttpEndpoint(port: 8000, env: "PORT")
    .WithExternalHttpEndpoints();
#pragma warning restore ASPIREHOSTINGPYTHON001

var apiService = builder.AddProject<Projects.ApiService>("apiservice")
    .WithReference(pythonApi)
    .WithReference(sqlServer)
    .WaitFor(sqlServer)
    .WaitFor(migrationService)
    .WithHttpHealthCheck("/health");

builder.AddNodeApp("web", "../web", "pnpm", ["dev"])
    .WithReference(apiService)
    .WithReference(pythonApi)
    .WithHttpEndpoint(port: 3000, name: "http")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();
