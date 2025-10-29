using Microsoft.EntityFrameworkCore;
using noMoreAzerty_back.Data;
using noMoreAzerty_back.Interfaces;
using noMoreAzerty_back.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using noMoreAzerty_back.UseCases.Vaults;
using MyApiProject.UseCases.Users;

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
// ----------------------------------------------------
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorization();

// ----------------------------------------------------
// 4️⃣ Enregistrement des dépendances custom
// ----------------------------------------------------
// 🧩 Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IVaultRepository, VaultRepository>(); // 👈 Ajout

// 🧠 Use Cases
builder.Services.AddScoped<GetOrCreateCurrentUserUseCase>();
builder.Services.AddScoped<GetAllVaultsUseCase>(); // 👈 Ajout

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
