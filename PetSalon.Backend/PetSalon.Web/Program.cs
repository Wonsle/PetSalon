using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using PetSalon.Models;
using PetSalon.Models.EntityModels;
using PetSalon.Models.DTOs;
using PetSalon.Services;
using PetSalon.Tools;

var builder = WebApplication.CreateBuilder(args);
AddDBServices(builder.Configuration, builder.Services);
AddServices(builder.Services);
AddJwtAuthentication(builder.Configuration, builder.Services);

// Configure file upload settings
builder.Services.Configure<FileUploadSettings>(builder.Configuration.GetSection("FileUpload"));

// Add services to the container.
builder.Services.AddSingleton<JwtHelpers>();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // 處理循環參考問題
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        // 設定 JSON 輸出格式
        options.JsonSerializerOptions.WriteIndented = true;
    });

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:3001", "http://localhost:3002", "http://localhost:3003", "http://127.0.0.1:3000", "http://127.0.0.1:3001", "http://127.0.0.1:3002", "http://127.0.0.1:3003")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials()
              .SetPreflightMaxAge(TimeSpan.FromSeconds(86400)); // 24 hours
    });

    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    // Use more permissive CORS in development
    app.UseCors("AllowAll");
}
else
{
    app.UseCors("AllowFrontend");
}

//app.UseHttpsRedirection();

// Enable static files serving for uploaded images
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();


void AddDBServices(IConfiguration configuration, IServiceCollection services)
{
    //MS SQL連線設定
    var conStrBuilder = new SqlConnectionStringBuilder(configuration.GetConnectionString("DefaultConnection"));
    var connection = conStrBuilder.ConnectionString;
    
    // Register the interceptor as a singleton for better performance in EF Core 8.0
    services.AddSingleton<EntitySaveChangesInterceptor>();
    
    services.AddDbContext<PetSalonContext>((serviceProvider, options) => {
        options.UseSqlServer(connection, sqlOptions => {
            // Enable retry on failure for better resilience
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorNumbersToAdd: null);
        });
        
        // Add the interceptor using dependency injection - EF Core 8.0 best practice
        var interceptor = serviceProvider.GetRequiredService<EntitySaveChangesInterceptor>();
        options.AddInterceptors(interceptor);
        
        // Enable detailed errors in development
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
        {
            options.EnableSensitiveDataLogging();
            options.EnableDetailedErrors();
        }
    });
}
void AddServices(IServiceCollection services)
{
    services.AddScoped<ICommonService, CommonService>();
    services.AddScoped<IPetService, PetService>();
    services.AddScoped<IContactPersonService, ContactPersonService>();
    services.AddScoped<IPetRelationService, PetRelationService>();
    services.AddScoped<ISubscriptionService, SubscriptionService>();
    services.AddScoped<IReservationService, PetSalon.Services.ReservationService>();
    services.AddScoped<PetSalon.Services.CodeTypeService.ICodeTypeService, PetSalon.Services.CodeTypeService.CodeTypeService>();
    
    // 新增的服務註冊
    services.AddScoped<IServiceService, ServiceService>();
    services.AddScoped<IPetServiceDurationService, PetServiceDurationService>();
    services.AddScoped<IServiceTypeService, ServiceTypeService>();
}

void AddJwtAuthentication(IConfiguration configuration, IServiceCollection services)
{
    var jwtSettings = configuration.GetSection("JwtSettings");
    var signKey = jwtSettings.GetValue<string>("SignKey");
    var issuer = jwtSettings.GetValue<string>("Issuer");

    // Validate required JWT configuration
    if (string.IsNullOrEmpty(signKey))
    {
        throw new InvalidOperationException("JWT SignKey is not configured. Please set JwtSettings:SignKey in configuration.");
    }

    if (string.IsNullOrEmpty(issuer))
    {
        throw new InvalidOperationException("JWT Issuer is not configured. Please set JwtSettings:Issuer in configuration.");
    }

    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signKey)),
                // .NET 8 best practice: Set clock skew to reduce token validation issues
                ClockSkew = TimeSpan.FromMinutes(5)
            };
        });
}