using OpenIddict.Api2;
using OpenIddict.Validation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("all", builder =>
    {
        builder.AllowAnyOrigin();
        builder.AllowAnyHeader();
        builder.AllowAnyMethod();
    });
});

builder.Services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiReader", policy =>
    {
        policy.RequireAuthenticatedUser();
        //policy.RequireClaim("scope", "api.read");
    });
});

builder.Services.AddOpenIddict()
    .AddValidation(opt =>
    {
        opt.SetIssuer("https://localhost:7279");
        opt.AddAudiences("api2");
        
        opt.UseSystemNetHttp();
        opt.UseAspNetCore();
    });

var app = builder.Build();

app.UseHttpsRedirection();
app.UseCors("all");

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/weatherforecast", (ILogger<Program> logger) =>
{
    logger.LogInformation("Get weatherforecast");
    var summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    return Results.Ok(Enumerable.Range(1, 5).Select(index => new WeatherForecast
    {
        Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
        TemperatureC = Random.Shared.Next(-20, 55),
        Summary = summaries[Random.Shared.Next(summaries.Length)]
    }).ToArray());
}).RequireAuthorization("ApiReader");

app.Run();