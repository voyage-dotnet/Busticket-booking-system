using BusTicketSystem.Web.Filters;
using BusTicketSystem.Web.Helper;
using BusTicketSystem.Web.Mapping;
using BusTicketSystem.Web.Middlewares;
using BusTicketSystem.Web.Filters;
using BusTicketSystem.Web.Helper;
using BusTicketSystem.Web.Mapping;
using BusTicketSystem.Web.Middlewares;
using BusTicketSystem.Web.Models;
using BusTicketSystem.Web.Repositories;
using BusTicketSystem.Web.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<BusTicketDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Auth service registrations
builder.Services.AddScoped<IAuthService, AuthService>();

using BusTicketSystem.Web.Repositories;
using BusTicketSystem.Web.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
builder.Services.AddScoped<IAgencyRepository, AgencyRepository>();
builder.Services.AddScoped<IAgencyService, AgencyService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddScoped<ValidateModelAttribute>();

builder.Services.AddAutoMapper(typeof(AgencyMappingProfile));

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<BusTicketSystem.Web.Validator.CreateBusRequestDTOValidator>();

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"]
             ?? builder.Configuration["JwtSettings:Key"]
             ?? builder.Configuration["Jwt:Secret"]
             ?? builder.Configuration["JwtSettings:Secret"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException("JWT key is missing in appsettings.json.");
builder.Services.AddDbContext<BusTicketDbContext>(options =>

builder.Services.AddAuthentication(options =>
builder.Services.AddScoped<IAgencyRepository, AgencyRepository>();
builder.Services.AddScoped<IAgencyService, AgencyService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

builder.Services.AddScoped<ValidateModelAttribute>();

builder.Services.AddAutoMapper(typeof(AgencyMappingProfile));

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<BusTicketSystem.Web.Validator.CreateBusRequestDTOValidator>();

{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
app.UseMiddleware<GlobalExceptionMiddleware>();

// Keep these in this order. Auth configuration can be added by Auth dev later.
app.UseAuthentication();
app.UseAuthorization();
    };
app.MapControllers();
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();
app.UseMiddleware<GlobalExceptionMiddleware>();


record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
app.Run();