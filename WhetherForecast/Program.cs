var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger'ı her ortamda açık et
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    // EKS'te kök path kullanıyorsan bu yeterli:
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Weather API v1");
    c.RoutePrefix = "swagger"; // -> /swagger
});

// (ALB arkasında redirect loop yaşarsan kapatabilirsin)
app.UseHttpsRedirection();

// Health (K8s için güzel)
app.MapGet("/healthz", () => Results.Ok(new { status = "ok" }));

// Kökü swagger'a yönlendir
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

var summaries = new[]
{
    "Freezing","Bracing","Chilly","Cool","Mild","Warm","Balmy","Hot","Sweltering","Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast(
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        )).ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
