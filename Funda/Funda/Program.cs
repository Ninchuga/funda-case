using Funda.Application.Services;
using Funda.DI;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddWebServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapGet("/listtoptensellingmakelaars", async (IMakelaarService makelaarService, string city = "Amsterdam", bool propertiesWithGarden = false) =>
{
    var topTenMakelaars = await makelaarService.GetTopTenSellingMakelaarsFor(city, propertiesWithGarden);

    return Results.Ok(topTenMakelaars);
})
.WithName("TopTenSellingMakelaars");

app.Run();