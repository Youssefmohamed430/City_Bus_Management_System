using City_Bus_Management_System;
using City_Bus_Management_System.DataLayer.Data;
using City_Bus_Management_System.DataLayer.Data.Config;
using City_Bus_Management_System.DataLayer.Entities;
using City_Bus_Management_System.Helpers;
using City_Bus_Management_System.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(cfg => cfg.AddDebug());

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.RegisterMapsterConfiguration();

builder.Services.AddCors(corsOptions =>

      corsOptions.AddPolicy("MyPolicy", CorsPolicy =>

      CorsPolicy.AllowAnyHeader()

      .AllowAnyMethod()

      .AllowAnyOrigin())
);

var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

var Connectionstring = config.GetSection("constr").Value;

builder.Services.AddDbContextPool<AppDbContext>(options =>
    options.UseSqlServer(Connectionstring)
);

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = true; // Add this line
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
}).AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 3;
    options.Lockout.AllowedForNewUsers = true;
});


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.RequireHttpsMetadata = false;
    o.SaveToken = false;
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = config["JWT:Issuer"],
        ValidAudience = config["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:Key"])),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));

builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddScoped<JWTService>();
builder.Services.AddScoped<IAuthService,AuthService>();
builder.Services.AddScoped<IAdminService,AdminService>();
builder.Services.AddScoped<IBusService,BusService>();
builder.Services.AddMemoryCache();


var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseCors("MyPolicy");

app.UseHttpsRedirection();

app.UseMiddleware<GlobalExceptionHandler>();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
