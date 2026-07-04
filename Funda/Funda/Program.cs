using Scalar.AspNetCore;
using Funda.Application.DI;
using Funda.DAL.DI;
using Funda.Application.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddApplicationServices();
builder.Services.AddDalServices(builder.Configuration);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapGet("/listtoptenmakelaars", async (IMakelaarService makelaarService, string city = "Amsterdam", bool propertiesWithGarden = false, int page = 1, int pageSize = 1000) =>
{
    var makelaars = await makelaarService.GetMakelaarsFor(city, propertiesWithGarden, Funda.Domain.Enums.PropertyType.Koop, page, pageSize);

    return Results.Ok("Here is response...");
})
.WithName("TopTenMakelaars");

app.Run();

internal record Makelaar(string Name, string City, int Rating, int TotalProperties);