using City_Bus_Management_System;
using City_Bus_Management_System.DataLayer.Data;
using City_Bus_Management_System.DataLayer.Data.Config;
using City_Bus_Management_System.DataLayer.Entities;
using City_Bus_Management_System.Helpers;
using City_Bus_Management_System.Hubs;
using City_Bus_Management_System.Services;
using Core_Layer;
using Data_Access_Layer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Service_Layer.IServices;
using Service_Layer.Services;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddRateLimiter(options =>
{
    options.AddSlidingWindowLimiter("sliding", opt =>
    {
        opt.PermitLimit = 10;               
        opt.Window = TimeSpan.FromMinutes(1); 
        opt.SegmentsPerWindow = 6;            
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0;                 
    });
});

builder.Services.AddLogging(cfg => cfg.AddDebug());

builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.DefaultIgnoreCondition =
            System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true; // Helpful for debugging
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.RegisterMapsterConfiguration();

builder.Services.AddCors(corsOptions =>

      corsOptions.AddPolicy("MyPolicy", CorsPolicy =>
          CorsPolicy.WithOrigins(
                "https://youssefmohamed430.github.io"
            )
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
));

var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

var Connectionstring = config.GetSection("constr").Value;

builder.Services.AddDbContextPool<AppDbContext>(options =>
    options.UseSqlServer(Connectionstring
    ));

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
builder.Services.AddScoped<ITripService,TripService>();
builder.Services.AddScoped<IScheduleService,ScheduleService>();
builder.Services.AddScoped<IStationService,StationService>();
builder.Services.AddScoped<ITicketService, TicketService>();
builder.Services.AddScoped<IWalletService, WalletService>();
builder.Services.AddScoped<IRouteService, RouteService>();
builder.Services.AddScoped<IDriverService, DriverService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
builder.Services.AddMemoryCache();


var app = builder.Build();

app.UseStaticFiles();

app.UseRateLimiter(); 

//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseCors("MyPolicy");

app.MapHub<TrackingHub>("/trackingHub");

app.UseHttpsRedirection();

app.UseMiddleware<GlobalExceptionHandler>();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
