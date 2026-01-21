using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using noMoreAzerty_back.Data;
using noMoreAzerty_back.Interfaces;
using noMoreAzerty_back.Interfaces.noMoreAzerty_back.Interfaces;
using noMoreAzerty_back.Interfaces.Services;
using noMoreAzerty_back.Middlewares; 
using noMoreAzerty_back.Repositories;
using noMoreAzerty_back.Service;
using noMoreAzerty_back.UseCases.Entries;
using noMoreAzerty_back.UseCases.Users;
using noMoreAzerty_back.UseCases.Vaults;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContextFactory<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddAuthorization();

builder.Services.AddScoped<IVaultEntryHistoryRepository, VaultEntryHistoryRepository>();
builder.Services.AddScoped<IVaultEntryHistoryService, VaultEntryHistoryService>();

builder.Services.AddScoped<IVaultEntryHistoryService, VaultEntryHistoryService>();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IVaultRepository, VaultRepository>();
builder.Services.AddScoped<IVaultEntryRepository, VaultEntryRepository>();

builder.Services.AddScoped<GetOrCreateCurrentUserUseCase>();

builder.Services.AddScoped<CreateVaultUseCase>();
builder.Services.AddScoped<DeleteVaultEntryUseCase>();
builder.Services.AddScoped<ValidateVaultAccessUseCase>();
builder.Services.AddScoped<UpdateVaultEntryUseCase>();

builder.Services.AddScoped<CreateVaultEntryUseCase>();
builder.Services.AddScoped<GetAllVaultsUseCase>();
builder.Services.AddScoped<GetSharedVaultsUseCase>();
builder.Services.AddScoped<GetUserVaultsUseCase>();
builder.Services.AddScoped<ShareVaultUseCase>();
builder.Services.AddScoped<UnshareVaultUseCase>();
builder.Services.AddScoped<UpdateVaultNameUseCase>();
builder.Services.AddScoped<DeleteVaultUseCase>();
builder.Services.AddScoped<GetVaultEntriesMetadataUseCase>();
builder.Services.AddScoped<GetVaultEntryByIdUseCase>();

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

app.UseMiddleware<EnsureUserProvisionedMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();

