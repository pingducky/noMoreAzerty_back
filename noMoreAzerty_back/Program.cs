using Microsoft.EntityFrameworkCore;
using noMoreAzerty_back.Data;
using noMoreAzerty_back.Interfaces;
using noMoreAzerty_back.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using noMoreAzerty_back.UseCases.Vaults;
using noMoreAzerty_back.UseCases.Entries;
using noMoreAzerty_back.UseCases.Users;
using noMoreAzerty_back.Middlewares; 

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorization();


builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IVaultRepository, VaultRepository>();
builder.Services.AddScoped<IVaultEntryRepository, VaultEntryRepository>();

builder.Services.AddScoped<GetOrCreateCurrentUserUseCase>();
builder.Services.AddScoped<GetAllVaultsUseCase>();
builder.Services.AddScoped<GetUserVaultsUseCase>();
builder.Services.AddScoped<GetSharedVaultsUseCase>();
builder.Services.AddScoped<CreateVaultUseCase>();
builder.Services.AddScoped<GetVaultEntriesUseCase>();


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

// System.AggregateException : 'Some services are not able to be constructed (Error while validating the service descriptor 'ServiceType: noMoreAzerty_back.UseCases.Entries.GetEntriesByVaultUseCase Lifetime: Scoped ImplementationType: noMoreAzerty_back.UseCases.Entries.GetEntriesByVaultUseCase': Unable to resolve service for type 'noMoreAzerty_back.Repositories.IVaultEntryRepository' while attempting to activate 'noMoreAzerty_back.UseCases.Entries.GetEntriesByVaultUseCase'.)'
var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

// ✅ Middleware de création/utilisateur automatique
app.UseMiddleware<EnsureUserProvisionedMiddleware>();

app.MapControllers();

// ----------------------------------------------------
// 9️⃣ Run App
// ----------------------------------------------------
app.Run();

