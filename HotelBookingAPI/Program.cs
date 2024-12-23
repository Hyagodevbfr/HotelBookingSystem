using HotelBookingAPI.Infra.Data;
using HotelBookingAPI.Infra.Data.Repositories;
using HotelBookingAPI.Mapping;
using HotelBookingAPI.Models;
using HotelBookingAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DevelopmentConnection"));
});

//Repositories
builder.Services.AddScoped<IAccount, AccountService>();
builder.Services.AddScoped<IRole, RoleService>();
builder.Services.AddScoped<IUserVerifier, UserVerifierService>();
builder.Services.AddScoped<IRoom, RoomService>();
builder.Services.AddScoped<ITraveler, TravelerService>();
builder.Services.AddScoped<ITravelerVerifier, TravelerVerifierService>();

builder.Services.AddAutoMapper(typeof(UserProfile));


//Identity
builder.Services.AddIdentity<AppUser,IdentityRole>( ).AddEntityFrameworkStores<AppDbContext>( ).AddDefaultTokenProviders( );

//Authetication
var jwtSetting = builder.Configuration.GetSection("JWTSetting");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidAudience = jwtSetting["ValidAudience"],
        ValidIssuer = jwtSetting["ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSetting.GetSection("securityKey").Value!))
    };
});



builder.Services.AddControllers( ).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter( ));
    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer( );
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer",new OpenApiSecurityScheme
    {
        Description = @"Jwt authorization example: 'Bearer 'eyJhbGciOiJIUzI1NiIsInR5c...'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement( ){
        {
            new OpenApiSecurityScheme{
                Reference = new OpenApiReference{
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "outh2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
} );

var app = builder.Build( );

// Configure the HTTP request pipeline.
if(app.Environment.IsDevelopment( ))
{
    app.UseSwagger( );
    app.UseSwaggerUI( );
}

app.UseHttpsRedirection( );

app.UseAuthorization( );

app.MapControllers( );

app.Run( );
