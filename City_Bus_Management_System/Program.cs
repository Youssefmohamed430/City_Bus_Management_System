using City_Bus_Management_System;
using City_Bus_Management_System.DataLayer.Data.Config;
using City_Bus_Management_System.Hubs;
using Service_Layer.ServiceRegistration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices(builder.Configuration);

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

app.UseStaticFiles();

app.UseRateLimiter();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("MyPolicy");

app.MapHub<TrackingHub>("/trackingHub");

app.UseHttpsRedirection();

app.UseMiddleware<GlobalExceptionHandler>();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();