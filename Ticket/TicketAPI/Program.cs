using System.Text;
using System.Text.Json;
using BusinessObject.Commons;
using BusinessObject.IService;
using BusinessObject.Mappers;
using BusinessObject.Service;
using CloudinaryDotNet;
using DataAccessObject.Entities;
using DataAccessObject.IRepo;
using DataAccessObject.Repo;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TicketAPI.Middleware;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var myConfig = new AppConfiguration();
configuration.Bind(myConfig);
builder.Services.AddSingleton(myConfig);
builder.Services.AddMemoryCache();

// Configure DbContext
builder.Services.AddDbContext<TicketContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DatabaseConnection")));

builder.Services.Configure<CloudinarySettings>(configuration.GetSection("Cloudinary"));
builder.Services.AddSingleton(provider =>
{
    var config = provider.GetRequiredService<IOptions<CloudinarySettings>>().Value;
    return new Cloudinary(new Account(
        config.CloudName,
        config.ApiKey,
        config.ApiSecret));
});

// Configure repositories
builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddScoped<IAttendeeRepo, AttendeeRepo>();
builder.Services.AddScoped<IEventRepo, EventRepo>();
builder.Services.AddScoped<IVenueRepo, VenueRepo>();
builder.Services.AddScoped<ITicketRepo, TicketRepo>();
builder.Services.AddScoped<IBoothRepo, BoothRepo>();
builder.Services.AddScoped<IPaymentRepo, PaymentRepo>();
builder.Services.AddScoped<ITransactionRepo, TransactionRepo>();
builder.Services.AddScoped<IBoothRequestRepo, BoothRequestRepo>();
// Configure services
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAttendeeService, AttendeeService>();
builder.Services.AddScoped<IEventService, EventService>();
builder.Services.AddScoped<IVenueService, VenueService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IBoothService, BoothService>();
builder.Services.AddScoped<IBoothRequestService, BoothRequestService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
// Configure AutoMapper
builder.Services.AddAutoMapper(typeof(MapperConfigurationsProfile));

// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSection = builder.Configuration.GetSection("JWTSection");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]))
        };
    });
// Configure Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
    options.AddPolicy("Staff", policy => policy.RequireRole("Staff"));
    options.AddPolicy("Sponsor", policy => policy.RequireRole("Sponsor"));
    options.AddPolicy("Organizer", policy => policy.RequireRole("Organizer"));
});

// Configure Swagger
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Ticket.API",
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. " +
                      "\n\nEnter your token in the text input below. " +
                      "\n\nExample: '12345abcde'",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("swagger/v1/swagger.json", "TicketApis v1");
    c.RoutePrefix = string.Empty;
});

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.MapControllers();

app.Run();