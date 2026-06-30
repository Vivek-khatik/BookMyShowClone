using System.Text;
using BookMyShow.Application.DTOs.Auth;
using BookMyShow.Application.DTOs.Bookings.Bookings;
using BookMyShow.Application.DTOs.Bookings.SeatLocks;
using BookMyShow.Application.DTOs.Movies;
using BookMyShow.Application.DTOs.Movies.Genres;
using BookMyShow.Application.DTOs.Movies.Languages;
using BookMyShow.Application.DTOs.Movies.Actors;
using BookMyShow.Application.DTOs.Shows.ShowPrices;
using BookMyShow.Application.DTOs.Shows.Shows;
using BookMyShow.Application.DTOs.Venues.Cities;
using BookMyShow.Application.DTOs.Venues.Venues;
using BookMyShow.Application.DTOs.Venues.Screens;
using BookMyShow.Application.DTOs.Venues.SeatCategories;
using BookMyShow.Application.DTOs.Venues.Seats;
using BookMyShow.Application.Interfaces;
using BookMyShow.Application.Validators.Auth;
using BookMyShow.Application.Validators.Bookings.Bookings;
using BookMyShow.Application.Validators.Bookings.SeatLocks;
using BookMyShow.Application.Validators.Movies;
using BookMyShow.Application.Validators.Movies.Genres;
using BookMyShow.Application.Validators.Movies.Languages;
using BookMyShow.Application.Validators.Movies.Actors;
using BookMyShow.Application.Validators.Shows.ShowPrices;
using BookMyShow.Application.Validators.Shows.Shows;
using BookMyShow.Application.Validators.Venues.Cities;
using BookMyShow.Application.Validators.Venues.Venues;
using BookMyShow.Application.Validators.Venues.Screens;
using BookMyShow.Application.Validators.Venues.SeatCategories;
using BookMyShow.Application.Validators.Venues.Seats;
using BookMyShow.Domain.Entities.Movies;
using BookMyShow.Infrastructure.Services;
using BookMyShow.Persistence.Context;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "BookMyShow API",
        Version = "v1"
    });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter JWT Bearer token as: Bearer {token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
    };

    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, securityScheme);
    options.AddSecurityRequirement(openApiDocument => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference(JwtBearerDefaults.AuthenticationScheme, openApiDocument, null),
            new List<string>()
        }
    });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddScoped<IVenueService, VenueService>();
builder.Services.AddScoped<IScreenService, ScreenService>();
builder.Services.AddScoped<ISeatCategoryService, SeatCategoryService>();
builder.Services.AddScoped<ISeatService, SeatService>();
builder.Services.AddScoped<IShowService, ShowService>();
builder.Services.AddScoped<IShowPriceService, ShowPriceService>();
builder.Services.AddScoped<ISeatLockService, SeatLockService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IGenreService, GenreService>();
builder.Services.AddScoped<ILanguageService, LanguageService>();
builder.Services.AddScoped<IActorService, ActorService>();

builder.Services.AddScoped<IValidator<RegisterRequest>, RegisterRequestValidator>();
builder.Services.AddScoped<IValidator<LoginRequest>, LoginRequestValidator>();
builder.Services.AddScoped<IValidator<RefreshTokenRequest>, RefreshTokenRequestValidator>();
builder.Services.AddScoped<IValidator<CreateMovieRequest>, CreateMovieRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateMovieRequest>, UpdateMovieRequestValidator>();
builder.Services.AddScoped<IValidator<CreateCityRequest>, CreateCityRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateCityRequest>, UpdateCityRequestValidator>();
builder.Services.AddScoped<IValidator<CreateVenueRequest>, CreateVenueRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateVenueRequest>, UpdateVenueRequestValidator>();
builder.Services.AddScoped<IValidator<CreateScreenRequest>, CreateScreenRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateScreenRequest>, UpdateScreenRequestValidator>();
builder.Services.AddScoped<IValidator<CreateSeatCategoryRequest>, CreateSeatCategoryRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateSeatCategoryRequest>, UpdateSeatCategoryRequestValidator>();
builder.Services.AddScoped<IValidator<CreateSeatRequest>, CreateSeatRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateSeatRequest>, UpdateSeatRequestValidator>();
builder.Services.AddScoped<IValidator<CreateShowRequest>, CreateShowRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateShowRequest>, UpdateShowRequestValidator>();
builder.Services.AddScoped<IValidator<CreateShowPriceRequest>, CreateShowPriceRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateShowPriceRequest>, UpdateShowPriceRequestValidator>();
builder.Services.AddScoped<IValidator<LockSeatsRequest>, LockSeatsRequestValidator>();
builder.Services.AddScoped<IValidator<CreateBookingRequest>, CreateBookingRequestValidator>();
builder.Services.AddScoped<IValidator<CreateGenreRequest>, CreateGenreRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateGenreRequest>, UpdateGenreRequestValidator>();
builder.Services.AddScoped<IValidator<CreateLanguageRequest>, CreateLanguageRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateLanguageRequest>, UpdateLanguageRequestValidator>();
builder.Services.AddScoped<IValidator<CreateActorRequest>, CreateActorRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateActorRequest>, UpdateActorRequestValidator>();

var jwtKey = builder.Configuration["Jwt:Key"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException("Jwt:Key is not configured.");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    if (!context.Languages.Any())
    {
        var languagesToSeed = new[]
        {
            "English", "Hindi", "Gujarati", "Tamil", "Telugu",
            "Malayalam", "Kannada", "Marathi", "Punjabi", "Bengali"
        };
        foreach (var lang in languagesToSeed)
        {
            context.Languages.Add(new Language { Name = lang, CreatedAt = DateTime.UtcNow });
        }
        context.SaveChanges();
    }
}

app.Run();
