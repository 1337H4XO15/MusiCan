using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MusiCan.Server.Data;
using MusiCan.Server.DatabaseContext;
using MusiCan.Server.Services;
using Serilog;
using System.Text;

const string version = "1.2.22";

Serilog.Log.Logger = new LoggerConfiguration()
.MinimumLevel.Information()
.MinimumLevel.Verbose()
.WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message}{NewLine}{Exception}")
.WriteTo.File("Logs/log-.txt",
    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message}{NewLine}{Exception}",
    rollingInterval: RollingInterval.Day,
    rollOnFileSizeLimit: true)
.CreateLogger();

Log.Information($"MusiCan {version} starting!");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// JsonWebToken Einstellungen hinzufügen
builder.Services.Configure<Jwt>(builder.Configuration.GetSection("Jwt"));



// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Datenbank hinzufügen
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// JsonWebToken (Bearer) Authentication hinzufügen
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        // Tokens laufen auch nach der angegeben Zeit aus und nicht erst 5 min später 
        ClockSkew = TimeSpan.Zero,

    };
});

// Authentication bereitstellen
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<DataContext>();

var app = builder.Build();

app.UseDefaultFiles();
app.MapStaticAssets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
