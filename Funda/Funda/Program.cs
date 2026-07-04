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

app.MapGet("/listtoptensellingmakelaars", async (IMakelaarService makelaarService, string city = "Amsterdam", bool propertiesWithGarden = false) =>
{
    var topTenMakelaars = await makelaarService.GetTopTenSellingMakelaarsFor(city, propertiesWithGarden);

    return Results.Ok(topTenMakelaars);
})
.WithName("TopTenSellingMakelaars");

app.Run();