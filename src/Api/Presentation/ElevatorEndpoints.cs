using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using ElevatorControl.Api.Application.Commands;
using ElevatorControl.Api.Application.Queries;
using ElevatorControl.Api.Application.Handlers;
using ElevatorControl.Api.Presentation.Models;

namespace ElevatorControl.Api.Presentation.Endpoints;

public static class ElevatorEndpoints
{
    public static WebApplication MapElevatorEndpoints(this WebApplication app)
    {
        app.MapPost("/api/elevators", async (CreateElevatorCommandHandler handler) =>
        {
            var result = await handler.HandleAsync(new CreateElevatorCommand());
            return Results.Created($"/api/elevators/{result.Id}", result);
        }).WithName("CreateElevator");

        app.MapGet("/api/elevators", async (GetElevatorsQueryHandler handler) =>
        {
            var elevators = await handler.HandleAsync(new GetElevatorsQuery());
            return Results.Ok(elevators);
        }).WithName("ListElevators");

        app.MapGet("/api/elevators/{id:int}/destinations", async (GetFloorDestinationsQueryHandler handler, int id) =>
        {
            var dests = await handler.HandleAsync(new GetFloorDestinationsQuery(id));
            return dests is null ? Results.NotFound() : Results.Ok(dests);
        }).WithName("GetDestinations");

        app.MapGet("/api/elevators/{id:int}/next", async (GetNextFloorQueryHandler handler, int id) =>
        {
            var nextFloor = await handler.HandleAsync(new GetNextFloorQuery(id));
            return nextFloor is null ? Results.NotFound() : Results.Ok(new { nextFloor });
        }).WithName("GetNextFloor");

        app.MapPost("/api/elevators/{id:int}/move", async (MoveToFloorCommandHandler handler, int id, FloorMoveRequest request) =>
        {
            if (request == null) return Results.BadRequest("Request body required");
            var ok = await handler.HandleAsync(new MoveToFloorCommand(id, request.Floor));
            return ok ? Results.Ok() : Results.NotFound($"Elevator {id} not found");
        });

        app.MapPost("/api/elevators/{id:int}/call", async (AddFloorCallCommandHandler handler, int id, FloorCallRequest request) =>
        {
            if (request == null) return Results.BadRequest();
            var ok = await handler.HandleAsync(new AddFloorCallCommand(id, request.Floor, request.Direction));
            return ok ? Results.Ok() : Results.NotFound();
        }).WithName("CallElevator");

        app.MapPost("/api/elevators/{id:int}/request", async (AddFloorDestinationCommandHandler handler, int id, FloorDestinationRequest request) =>
        {
            if (request == null) return Results.BadRequest();
            var ok = await handler.HandleAsync(new AddFloorDestinationCommand(id, request.Floor));
            return ok ? Results.Ok() : Results.NotFound();
        }).WithName("RequestFloor");

        return app;
    }
}
