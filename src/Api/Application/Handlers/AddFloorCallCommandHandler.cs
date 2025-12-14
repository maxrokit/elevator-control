using System.Threading.Tasks;
using ElevatorControl.Api.Application.Commands;
using ElevatorControl.Api.Infrastructure.Repositories;

namespace ElevatorControl.Api.Application.Handlers;

public class AddFloorCallCommandHandler
{
    private readonly ILogger<AddFloorCallCommandHandler> _logger;
    private readonly IElevatorRepository _repository;

    public AddFloorCallCommandHandler(IElevatorRepository repository, ILogger<AddFloorCallCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<bool> HandleAsync(AddFloorCallCommand command)
    {
        var elevator = await _repository.GetByIdAsync(command.ElevatorId);
        if (elevator == null) return false;

        elevator.AddFloorCall(command.Floor, command.Direction);

        _logger.LogInformation("Added floor call from floor {Floor} going {Direction} to elevator {Id}",
            command.Floor, command.Direction, command.ElevatorId);

        return true;
    }
}
