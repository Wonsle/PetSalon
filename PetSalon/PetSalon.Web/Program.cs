using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PetSalon.Models;
using PetSalon.Models.EntityModels;
using PetSalon.Services;
using PetSalon.Tools;

var builder = WebApplication.CreateBuilder(args);
AddDBServices(builder.Configuration, builder.Services);
AddServices(builder.Services);
// Add services to the container.
builder.Services.AddSingleton<JwtHelpers>();
builder.Services.AddControllers();
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

//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();


void AddDBServices(IConfiguration configuration, IServiceCollection services)
{
    //MS SQL¸ê®Æ®w
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
}