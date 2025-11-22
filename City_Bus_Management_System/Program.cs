using City_Bus_Management_System;
using City_Bus_Management_System.DataLayer.Data.Config;
using City_Bus_Management_System.Hubs;
using DotNetEnv;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.OpenApi.Models;
using Serilog;
using Service_Layer.ServiceRegistration;
using Service_Layer.Services;
using System.Text;
try
{
    Log.Logger.Information("Starting up the application...");

    var builder = WebApplication.CreateBuilder(args);

    Env.Load();

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

    builder.Services.AddHttpClient();


    builder.Services.AddHangfireServer();

    builder.Host.UseSerilog((context, services, configuration) =>
        configuration.ReadFrom.Configuration(context.Configuration)
                     .Enrich.FromLogContext()
                     .WriteTo.Console());

    builder.Services.AddApplicationServices(builder.Configuration);
    builder.Services.AddScoped<INotificationHubService, NotificationHub>();

    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "CityBus API",
            Version = "v1"
        });

        options.CustomSchemaIds(type => type.FullName);
    });
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

    // قبل app.MapControllers()
    app.Use(async (context, next) =>
    {
        if (context.Request.Path.StartsWithSegments("/Wallet/callback"))
        {
            Log.Information("========================================");
            Log.Information("=== INCOMING PAYMOB CALLBACK REQUEST ===");
            Log.Information("========================================");
            Log.Information("Time: {Time}", DateTime.UtcNow);
            Log.Information("Method: {Method}", context.Request.Method);
            Log.Information("Path: {Path}", context.Request.Path);
            Log.Information("Query: {Query}", context.Request.QueryString);

            Log.Information("--- Headers ---");
            foreach (var header in context.Request.Headers)
            {
                Log.Information("{Key}: {Value}", header.Key, header.Value);
            }

            context.Request.EnableBuffering();
            using var reader = new StreamReader(
                context.Request.Body,
                encoding: Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                leaveOpen: true);

            var body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;

            Log.Information("--- Request Body ---");
            Log.Information("{Body}", body);
            Log.Information("========================================");

            // ✅ Capture the response body
            var originalBodyStream = context.Response.Body;

            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            try
            {
                await next();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "=== EXCEPTION DURING REQUEST PROCESSING ===");
                Log.Error("Exception Type: {Type}", ex.GetType().Name);
                Log.Error("Exception Message: {Message}", ex.Message);
                Log.Error("Stack Trace: {StackTrace}", ex.StackTrace);
                throw;
            }

            // ✅ Read response
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            Log.Information("=== RESPONSE ===");
            Log.Information("Status Code: {StatusCode}", context.Response.StatusCode);

            if (context.Response.StatusCode == 400)
            {
                Log.Warning("🔴 === 400 BAD REQUEST DETECTED ===");
                Log.Warning("Response Body: {ResponseBody}", responseText);

                // ✅ Check ModelState errors if available
                if (context.Items.ContainsKey("ModelStateErrors"))
                {
                    Log.Warning("Model State Errors: {Errors}", context.Items["ModelStateErrors"]);
                }
            }
            else if (context.Response.StatusCode >= 500)
            {
                Log.Error("🔴 === SERVER ERROR {StatusCode} ===", context.Response.StatusCode);
                Log.Error("Response Body: {ResponseBody}", responseText);
            }
            else if (context.Response.StatusCode == 200)
            {
                Log.Information("✅ === SUCCESS 200 ===");
                Log.Information("Response Body: {ResponseBody}", responseText);
            }

            Log.Information("========================================");

            // ✅ Copy response back to original stream
            await responseBody.CopyToAsync(originalBodyStream);
        }
        else
        {
            await next();
        }
    });

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



