using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
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
using Net.payOS;
using TicketAPI.Filters;
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
builder.Services.AddSingleton(x =>
    new PaypalClient(
        builder.Configuration["PayPalOptions:ClientId"],
        builder.Configuration["PayPalOptions:ClientSecret"],
        builder.Configuration["PayPalOptions:Mode"]
    )
);

var payOs = new PayOS(
    configuration["Environment:PAYOS_CLIENT_ID"] ?? throw new Exception("Cannot find environment client"),
    configuration["Environment:PAYOS_API_KEY"] ?? throw new Exception("Cannot find environment api"),
    configuration["Environment:PAYOS_CHECKSUM_KEY"] ?? throw new Exception("Cannot find environment sum"));
builder.Services.AddScoped<PayOS>(_ => payOs);

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
builder.Services.AddScoped<IGiftRepo, GiftRepo>();
builder.Services.AddScoped<IGiftReceptionRepo, GiftReceptionRepo>();
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
builder.Services.AddScoped<IGiftService, GiftService>();
builder.Services.AddScoped<IGiftReceptionService, GiftReceptionService>();
// Configure AutoMapper
builder.Services.AddAutoMapper(typeof(MapperConfigurationsProfile));

// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var config = (IConfiguration)configuration;
        var jwtSection = builder.Configuration.GetSection("JWTSection");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWTSection:Key"])),
            RoleClaimType = ClaimTypes.Role
        };
    });
// Configure Swagger
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.WriteIndented = true;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Ticket.API",
    });
    c.OperationFilter<DefaultResponseOperationFilter>();
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
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
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
app.UseHsts();
app.UseExceptionHandlingMiddleware();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("Allow");


app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.MapControllers();

app.Run();