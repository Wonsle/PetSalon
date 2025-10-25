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

// Configure CORS from settings
builder.Services.AddCors(options =>
{
    options.AddPolicy("ApiCorsPolicy", policy =>
    {
        // 從設定檔讀取允許的來源
        var allowedOrigins = builder.Configuration
            .GetSection("CorsSettings:AllowedOrigins")
            .Get<string[]>() ?? Array.Empty<string>();

        if (allowedOrigins.Length == 0)
        {
            throw new InvalidOperationException(
                "CORS AllowedOrigins must be configured in appsettings.json");
        }

        policy.WithOrigins(allowedOrigins);

        // 設定允許的 Methods
        var allowedMethods = builder.Configuration
            .GetSection("CorsSettings:AllowedMethods")
            .Get<string[]>();

        if (allowedMethods != null && allowedMethods.Length > 0)
        {
            policy.WithMethods(allowedMethods);
        }
        else
        {
            policy.AllowAnyMethod();
        }

        // 設定允許的 Headers
        var allowedHeaders = builder.Configuration
            .GetSection("CorsSettings:AllowedHeaders")
            .Get<string[]>();

        if (allowedHeaders != null && allowedHeaders.Length > 0)
        {
            policy.WithHeaders(allowedHeaders);
        }
        else
        {
            policy.AllowAnyHeader();
        }

        // 設定 Credentials
        var allowCredentials = builder.Configuration
            .GetValue<bool>("CorsSettings:AllowCredentials", true);

        if (allowCredentials)
        {
            policy.AllowCredentials();
        }

        // 設定 Preflight Cache (16小時 = 57600秒)
        var preflightMaxAge = builder.Configuration
            .GetValue<int>("CorsSettings:PreflightMaxAge", 57600);

        policy.SetPreflightMaxAge(TimeSpan.FromSeconds(preflightMaxAge));

        // Development 環境額外的來源驗證
        if (builder.Environment.IsDevelopment())
        {
            policy.SetIsOriginAllowed(origin =>
            {
                if (string.IsNullOrEmpty(origin)) return false;
                var uri = new Uri(origin);
                // 在開發環境允許 localhost 和 127.0.0.1
                return uri.Host == "localhost" || uri.Host == "127.0.0.1";
            });
        }
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
}

// 統一使用 ApiCorsPolicy（環境別設定已在 appsettings 中區分）
app.UseCors("ApiCorsPolicy");

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
        options.UseSqlServer(connection);

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
    services.AddScoped<IPetServicePriceService, PetServicePriceService>();  // 整合價格和時長管理（取代舊的 PetServiceDurationService）
    services.AddScoped<IServiceTypeService, ServiceTypeService>();

    // 檔案服務註冊
    services.AddScoped<IFileService, FileService>();
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