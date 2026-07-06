using Funda.Application.Services;
using Funda.DI;
using Scalar.AspNetCore;
using Serilog;

// Setup a temporary bootstrap logger for application startup
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting up the Web API...");

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddWebServices(builder.Configuration);

    var app = builder.Build();

    // Middleware that reduces the chatty default Microsoft request logs into a single clean summary line on startup
    app.UseSerilogRequestLogging();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference();
    }

    app.UseHttpsRedirection();

    app.MapGet("/listtoptensellingmakelaars", async (IMakelaarService makelaarService, CancellationToken cancellationToken, string city = "Amsterdam", bool propertiesWithGarden = false) =>
    {
        var topTenMakelaarsResult = await makelaarService.GetTopTenSellingMakelaarsFor(city, propertiesWithGarden, cancellationToken);

        return Results.Ok(topTenMakelaarsResult);
    })
    .WithName("TopTenSellingMakelaars")
    .WithSummary("Top Ten Selling Makelaars")
    .WithDescription("Retrieves the top 10 real estate agents based on sales volume.")
    .WithTags("Makelaars"); // <-- Groups it in the Scalar sidebar;

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "The application failed to start correctly.");
}
finally
{
    Log.CloseAndFlush(); // Ensures all logs are written before the app shuts down
}