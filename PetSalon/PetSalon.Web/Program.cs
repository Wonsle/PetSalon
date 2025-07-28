using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using PetSalon.Models;
using PetSalon.Models.EntityModels;
using PetSalon.Services;
using PetSalon.Tools;

var builder = WebApplication.CreateBuilder(args);
AddDBServices(builder.Configuration, builder.Services);
AddServices(builder.Services);
AddJwtAuthentication(builder.Configuration, builder.Services);

// Add services to the container.
builder.Services.AddSingleton<JwtHelpers>();
builder.Services.AddControllers();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();


void AddDBServices(IConfiguration configuration, IServiceCollection services)
{
    //MS SQL��Ʈw
    var conStrBuilder = new SqlConnectionStringBuilder(configuration.GetConnectionString("DefaultConnection"))
;
    var connection = conStrBuilder.ConnectionString;
    services.AddDbContext<PetSalonContext>(options => options.UseSqlServer(connection)
                                                            .AddInterceptors(new EntitySaveChangesInterceptor())
                                            );
}
void AddServices(IServiceCollection services)
{
    services.AddScoped<ICommonService, CommonService>();
    services.AddScoped<IPetService, PetService>();
    services.AddScoped<IContactPersonService, ContactPersonService>();
    services.AddScoped<ISubscriptionService, SubscriptionService>();
    services.AddScoped<IReservationService, ReservationService>();
}

void AddJwtAuthentication(IConfiguration configuration, IServiceCollection services)
{
    var jwtSettings = configuration.GetSection("JwtSettings");
    var signKey = jwtSettings.GetValue<string>("SignKey");
    
    services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.GetValue<string>("Issuer"),
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signKey))
            };
        });
}