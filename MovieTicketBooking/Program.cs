
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MovieTicketBooking.Data;
using MovieTicketBooking.Middleware;
using MovieTicketBooking.Models;
using MovieTicketBooking.Repositories;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services;
using MovieTicketBooking.Services.Interfaces;
using Scalar.AspNetCore;
using System.Text;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// 1. Add Controller and OpenAPI services
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// Enable CORS for Angular frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 2. Configure SQL Server Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration
        .GetConnectionString("DefaultConnection");

    options.UseSqlServer(connectionString);
});

// 3. Register Repositories
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<ITheatreRepository, TheatreRepository>();
builder.Services.AddScoped<IScreenRepository, ScreenRepository>();
builder.Services.AddScoped<ISeatRepository, SeatRepository>();
builder.Services.AddScoped<IShowRepository, ShowRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();


// 4. Register Application Services
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<ITheatreService, TheatreService>();
builder.Services.AddScoped<IScreenService, ScreenService>();
builder.Services.AddScoped<ISeatService, SeatService>();
builder.Services.AddScoped<IShowService, ShowService>();
builder.Services.AddScoped<IBookingService, BookingService>();

// 5. Register Password Hashing Services
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IPasswordService, PasswordService>();

// 6. Register JWT Token Service
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// 7. Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");

var secretKey = jwtSettings["Key"];

if (string.IsNullOrWhiteSpace(secretKey) ||
    Encoding.UTF8.GetByteCount(secretKey) < 32)
{
    throw new InvalidOperationException(
        "JWT signing key must contain at least 32 bytes."
    );
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtSettings["Issuer"],

                ValidateAudience = true,
                ValidAudience = jwtSettings["Audience"],

                ValidateLifetime = true,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(secretKey)
                ),

                RoleClaimType = ClaimTypes.Role,

                ClockSkew = TimeSpan.Zero
            };
    });

// 8. Enable Authorization
builder.Services.AddAuthorization();

// 9. Build Application
var app = builder.Build();

// 10. Create Admin Account (One-Time Setup)
var adminEmail = app.Configuration["AdminSeed:Email"];
var adminPassword = app.Configuration["AdminSeed:Password"];

if (!string.IsNullOrWhiteSpace(adminEmail) &&
    !string.IsNullOrWhiteSpace(adminPassword))
{
    using (var scope = app.Services.CreateScope())
    {
        var userRepository = scope.ServiceProvider
            .GetRequiredService<IUserRepository>();

        var passwordService = scope.ServiceProvider
            .GetRequiredService<IPasswordService>();

        string email = adminEmail.Trim().ToLowerInvariant();

        var existingAdmin =
            await userRepository.GetByEmailAsync(email);

        if (existingAdmin == null)
        {
            var admin = new User
            {
                FullName = "System Administrator",
                Email = email,
                Role = "Admin",
                CreatedAt = DateTime.UtcNow
            };

            admin.PasswordHash =
                passwordService.HashPassword(admin, adminPassword);

            await userRepository.AddAsync(admin);

            Console.WriteLine("Admin account created successfully.");
        }
        else if (!string.Equals(
            existingAdmin.Role,
            "Admin",
            StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "The configured Admin email belongs to a non-Admin user."
            );
        }
        else
        {
            Console.WriteLine("Admin account already exists.");
        }
    }
}

// 10. Configure Middleware

// Global Exception Handling
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseCors("AllowAngularApp");

// Authentication must run before Authorization
app.UseAuthentication();
app.UseAuthorization();

// Map Controllers
app.MapControllers();

// 11. Run Application
app.Run();
