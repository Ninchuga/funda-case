using Scalar.AspNetCore;
using Funda.Application.DI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddApplicationServices();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapGet("/listtoptenmakelaars", (string city = "Amsterdam", bool propertiesWithGarden = false) =>
{

    return Results.Ok("Here is response...");
})
.WithName("TopTenMakelaars");

app.Run();

internal record Makelaar(string Name, string City, int Rating, int TotalProperties);