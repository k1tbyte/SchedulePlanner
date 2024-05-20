using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;
using SchedulePlanner.Backend.Data;
using SchedulePlanner.Backend.Data.Models;
using SchedulePlanner.Backend.Repositories;
using SchedulePlanner.Backend.Repositories.Abstraction;
using SchedulePlanner.Backend.Services;

namespace SchedulePlanner.Backend;

internal static class App
{
    public const string RoutePattern = "api/v1/[controller]/[action]";
    private static WebApplication Instance = null!;
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
        });
        builder.Services.Configure<RouteOptions>(o => o.LowercaseUrls = true);
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddHttpContextAccessor();
        builder.Services.AddDbContext<AppDbContext>();
        builder.Services.AddScoped<JwtService>();
        builder.Services.AddScoped<IUserRepository,UserRepository>();
        builder.Services.AddScoped<IDepartmentRepository,DepartmentRepository>();
        builder.Services.AddScoped<ICrudRepository<Speciality>,SpecialityRepository>();
        builder.Services.AddScoped<ICrudRepository<Group>,GroupRepository>();

        ConfigureAuthentication(builder);
        Instance = builder.Build();

// Configure the HTTP request pipeline.
        if (Instance.Environment.IsDevelopment())
        {
            Instance.UseSwagger();
            Instance.UseSwaggerUI();
        }

        Instance.UseHttpsRedirection();
        Instance.MapControllers();
        Instance.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                               ForwardedHeaders.XForwardedProto
        });
        Instance.Run();
    }

    private static void ConfigureAuthentication(WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(o =>
        {
            o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultScheme             = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o =>
        {
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer              =  Instance.Configuration["JwtSettings:Issuer"],
                IssuerSigningKey         = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(Instance.Configuration["JwtSettings:Key"]!)),
                ValidateIssuer           = true,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                ValidateLifetime         = true,
            };
        });
    }
}