using City_Bus_Management_System;
using City_Bus_Management_System.DataLayer.Data.Config;
using City_Bus_Management_System.Hubs;
using Serilog;
using Service_Layer.ServiceRegistration;

var builder = WebApplication.CreateBuilder(args);


builder.Host.UseSerilog((context, services, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration)
                 .Enrich.FromLogContext()
                 .WriteTo.Console());


builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddScoped<INotificationHubService, NotificationHub>();


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

builder.Services.AddMemoryCache();

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseStaticFiles();

app.UseRateLimiter();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("MyPolicy");

app.MapHub<TrackingHub>("/trackingHub");
app.MapHub<NotificationHub>("/notificationHub");

app.MapPost("/Booking", (BookingDTO dookingdto, IBookingService bookingService) =>
{
    bookingService.BookTicket(dookingdto);
    return Results.Ok("Booking created!");
})
.RequireRateLimiting("Sliding"); 


app.UseHttpsRedirection();

app.UseMiddleware<GlobalExceptionHandler>();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

try
{
    Log.Logger.Information("Application Started Successfully");
}
catch (Exception ex)
{
    Log.Logger.Fatal(ex, "Application Failed to Start");
}

