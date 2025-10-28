using Microsoft.EntityFrameworkCore;
using MyApiProject.Data;
using MyApiProject.Interfaces;
using MyApiProject.Repositories;
using MyApiProject.UseCases.Users;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;


var builder = WebApplication.CreateBuilder(args);

// ----------------------------------------------------
// 1️⃣ Configuration de base
// ----------------------------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ----------------------------------------------------
// 2️⃣ Enregistrement du DbContext
// ----------------------------------------------------
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ----------------------------------------------------
// 3️⃣ Configuration Authentication / Authorization
// (nécessaire si tu utilises [Authorize])
// ----------------------------------------------------
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorization();

// ----------------------------------------------------
// 4️⃣ Enregistrement des dépendances custom
// ----------------------------------------------------
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<GetOrCreateCurrentUserUseCase>();

// ----------------------------------------------------
// 5️⃣ Controllers & CORS
// ----------------------------------------------------
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ----------------------------------------------------
// 6️⃣ Build App
// ----------------------------------------------------
var app = builder.Build();

// ----------------------------------------------------
// 7️⃣ Middleware
// ----------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ----------------------------------------------------
// 8️⃣ Exemple endpoint "WeatherForecast"
// ----------------------------------------------------
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild",
    "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast(
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

// ----------------------------------------------------
// 9️⃣ Run App
// ----------------------------------------------------
app.Run();

// ----------------------------------------------------
// 🔹 Record pour l'exemple WeatherForecast
// ----------------------------------------------------
internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
