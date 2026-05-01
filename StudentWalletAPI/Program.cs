using Microsoft.EntityFrameworkCore;
using StudentWalletAPI.Data;
using StudentWalletAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllers();

// Add Entity Framework
builder.Services.AddDbContext<WalletDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IWalletService, WalletService>();

// Add OpenAPI (Modern Swagger replacement for .NET 10)
builder.Services.AddOpenApi();

// Add Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "SecretKeyForStudentWalletAPI2024";
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Enable OpenAPI and Scalar UI
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

// Auto-apply migrations and create database
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<WalletDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        context.Database.EnsureCreated();
        logger.LogInformation("Database created successfully");
        
        if (!context.Students.Any())
        {
            var students = new[]
            {
                new StudentWalletAPI.Models.Student { StudentId = "STU001", Name = "John Doe", PIN = "1234", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new StudentWalletAPI.Models.Student { StudentId = "STU002", Name = "Jane Smith", PIN = "5678", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new StudentWalletAPI.Models.Student { StudentId = "STU003", Name = "Mike Johnson", PIN = "9012", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            };

            context.Students.AddRange(students);
            context.SaveChanges();

            var wallets = new[]
            {
                new StudentWalletAPI.Models.Wallet { WalletId = "WAL001", StudentId = "STU001", Balance = 100.00m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new StudentWalletAPI.Models.Wallet { WalletId = "WAL002", StudentId = "STU002", Balance = 250.50m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new StudentWalletAPI.Models.Wallet { WalletId = "WAL003", StudentId = "STU003", Balance = 75.25m, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
            };

            context.Wallets.AddRange(wallets);
            context.SaveChanges();
            
            logger.LogInformation("Seeded initial students and wallets");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while creating the database");
    }
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();