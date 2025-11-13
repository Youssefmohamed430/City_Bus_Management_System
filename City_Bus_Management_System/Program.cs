using City_Bus_Management_System;
using City_Bus_Management_System.DataLayer.Data.Config;
using City_Bus_Management_System.Hubs;
using Hangfire;
using Hangfire.SqlServer;
using Serilog;
using Service_Layer.ServiceRegistration;
try
{
    Log.Logger.Information("Starting up the application...");

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddHangfire(configuration => configuration
                    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings()
                    .UseSqlServerStorage(builder.Configuration.GetSection("constr").Value,
                        new SqlServerStorageOptions
                        {
                            CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                            SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                            QueuePollInterval = TimeSpan.Zero,
                            UseRecommendedIsolationLevel = true,
                            DisableGlobalLocks = true
                        }));

    builder.Services.AddHangfireServer();

    builder.Host.UseSerilog((context, services, configuration) =>
        configuration.ReadFrom.Configuration(context.Configuration)
                     .Enrich.FromLogContext()
                     .WriteTo.Console());

    builder.Services.AddApplicationServices(builder.Configuration);
    builder.Services.AddScoped<INotificationHubService, NotificationHub>();

    builder.Services.AddSwaggerGen();

    builder.Services.RegisterMapsterConfiguration();

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

    app.UseHangfireDashboard("/hangfire");

    BackgroundJobsAddition.AddBackgroundJobServices();

    app.MapControllers();

    Log.Logger.Information("Application Started Successfully");

    app.Run();

}
catch (Exception ex)
{
    Log.Logger.Fatal(ex, "Application Failed to Start");
}
finally
{
    Log.CloseAndFlush();
}



