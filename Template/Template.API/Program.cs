using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Template.API;
using Template.API.Authorization;
using Template.API.Dashboard.Events;
using Template.API.DependencyInjection;
using Template.API.Extensions;
using Template.API.Swagger.AppBuilder;
using Template.Application.Interfaces;
using Template.Dashboard.Interfaces;
using Template.Dashboard.Events;
using Template.Dashboard.Events.Handlers;
using Template.Domain.Entities;
using Template.Domain.Entities.Security;
using Template.Domain.Events.Employee;
using Template.Domain.Events.Lab;
using Template.Domain.Events.Notification;
using Template.Application.Events.Handlers;
using Template.Dashboard.Common.Interfaces;
using Template.Infrastructe.Services;
using Template.Infrastructe.Events.RabbitMQ;
using Template.Persistence.DbContext;
using Template.Domain.Primitives.Entity.Identity;
using Template.Infrastructe;
using Template.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddNeptuneeSwagger(o =>
    o.SwaggerDocs<SampleApiGroup>()
        .GroupNamesDocInclusion(escapeDocs: SampleApiGroup.All.ToString())
        .AddJwtBearerSecurityScheme());


// Add Authorization
builder.Services.AddScoped(typeof(IRepository),typeof(Repository));
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(IAuthService<>).Assembly);
});
builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionsHandler>();


// Register Services
builder.Services.AddInfrastructe()
    .AddPersistence(builder.Configuration);

// Register HttpClient for FCM
builder.Services.AddHttpClient("FCM");

// Configure RabbitMQ Settings
builder.Services.Configure<RabbitMQSettings>(
    builder.Configuration.GetSection(RabbitMQSettings.SectionName));

// Register Event Bus
builder.Services.AddSingleton<IEventBus, RabbitMQEventBus>();
builder.Services.AddSignalR();
// Register Integration Event Handlers
builder.Services.AddScoped<EmployeeCreatedIntegrationEventHandler>();
builder.Services.AddScoped<EmployeeDeletedIntegrationEventHandler>();
builder.Services.AddScoped<LabCreatedIntegrationEventHandler>();

// Register FCM Notification Event Handlers
builder.Services.AddScoped<SendNotificationIntegrationEventHandler>();

// Add Controllers
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});
var app = builder.Build();
app.UseCors("AllowFrontend");
// Configure Event Bus Subscriptions
var eventBus = app.Services.GetRequiredService<IEventBus>();
eventBus.Subscribe<EmployeeCreatedIntegrationEvent, EmployeeCreatedIntegrationEventHandler>();
eventBus.Subscribe<EmployeeDeletedIntegrationEvent, EmployeeDeletedIntegrationEventHandler>();
eventBus.Subscribe<LabCreatedIntegrationEvent, LabCreatedIntegrationEventHandler>();

// Start consuming events (optional - only if you want to consume events from other services)
// Uncomment the line below to start consuming events
eventBus.StartConsuming();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCustomSwagger(options =>
    {
        options.AddEndpoints<SampleApiGroup>();
        options.SetDocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
