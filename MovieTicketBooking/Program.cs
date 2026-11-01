using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.Middleware;
using MovieTicketBooking.Repositories;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services;
using MovieTicketBooking.Services.Interfaces;
using Scalar.AspNetCore;

// 1. Create the WebApplicationBuilder which sets up configuration, logging, and services
var builder = WebApplication.CreateBuilder(args);

// 2. Add services to the Dependency Injection (DI) container.

// Enable Controller support for our Web API endpoints
builder.Services.AddControllers();

// Register OpenAPI generation service
builder.Services.AddOpenApi();

// Register ApplicationDbContext with SQL Server using the connection string from appsettings.json
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);
});

// Register Repositories (Scoped lifetime: one instance per HTTP request)
builder.Services.AddScoped<IMovieRepository, MovieRepository>();
builder.Services.AddScoped<ITheatreRepository, TheatreRepository>();
builder.Services.AddScoped<IScreenRepository, ScreenRepository>();
builder.Services.AddScoped<ISeatRepository, SeatRepository>();
builder.Services.AddScoped<IShowRepository, ShowRepository>();
builder.Services.AddScoped<IBookingRepository, BookingRepository>();

// Register Services (Scoped lifetime: one instance per HTTP request)
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<ITheatreService, TheatreService>();
builder.Services.AddScoped<IScreenService, ScreenService>();
builder.Services.AddScoped<ISeatService, SeatService>();
builder.Services.AddScoped<IShowService, ShowService>();
builder.Services.AddScoped<IBookingService, BookingService>();

// 3. Build the WebApplication
var app = builder.Build();

// 4. Configure the HTTP request processing pipeline (Middleware).

// Centralized Exception Handling Middleware: Catches any unhandled exceptions globally
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    // Generates the OpenAPI / Swagger JSON document
    app.MapOpenApi();

    // Enables interactive Scalar API documentation UI at /scalar/v1
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Maps attribute-routed controllers (e.g., [Route("api/[controller]")]) to HTTP endpoints
app.MapControllers();

// 5. Start listening for incoming HTTP requests
app.Run();
