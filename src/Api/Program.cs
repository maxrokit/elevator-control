using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

// Project namespaces for endpoints, services and models
using ElevatorControl.Api.Presentation.Endpoints;
using ElevatorControl.Api.Infrastructure.Repositories;
using ElevatorControl.Api.Application.Handlers;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel URL from environment or default to port 8080 for local testing
var urls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS") ?? "http://localhost:8080";
builder.WebHost.UseUrls(urls);

// Add minimal API support
builder.Services.AddEndpointsApiExplorer();
// Only add Swagger/OpenAPI generation in Development
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddSwaggerGen();
}

// Add CORS support for client applications
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Health checks: register dependency checks here (DB, caches, etc.)
builder.Services.AddHealthChecks();

// Register repository and handlers
builder.Services.AddSingleton<IElevatorRepository, ElevatorRepository>();

// Register command handlers
builder.Services.AddScoped<CreateElevatorCommandHandler>();
builder.Services.AddScoped<AddFloorDestinationCommandHandler>();
builder.Services.AddScoped<AddFloorCallCommandHandler>();
builder.Services.AddScoped<MoveToFloorCommandHandler>();

// Register query handlers
builder.Services.AddScoped<GetElevatorsQueryHandler>();
builder.Services.AddScoped<GetFloorDestinationsQueryHandler>();
builder.Services.AddScoped<GetNextFloorQueryHandler>();

var app = builder.Build();

// Enable static files (serves wwwroot/index.html as UI)
app.UseStaticFiles();
app.UseDefaultFiles();

// Enable CORS
app.UseCors();

// Enable Swagger only in Development environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// Map elevator endpoints from separate module
app.MapElevatorEndpoints();

app.Run();
