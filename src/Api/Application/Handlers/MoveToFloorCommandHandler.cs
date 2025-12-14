using System.Threading.Tasks;
using ElevatorControl.Api.Application.Commands;
using ElevatorControl.Api.Infrastructure.Repositories;

namespace ElevatorControl.Api.Application.Handlers;

public class MoveToFloorCommandHandler
{
    private readonly ILogger<MoveToFloorCommandHandler> _logger;
    private readonly IElevatorRepository _repository;

    public MoveToFloorCommandHandler(IElevatorRepository repository, ILogger<MoveToFloorCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<bool> HandleAsync(MoveToFloorCommand command)
    {
        var elevator = await _repository.GetByIdAsync(command.ElevatorId);
        if (elevator == null) return false;

        _logger.LogInformation("Moving elevator {Id} to floor {Floor}",
            command.ElevatorId, command.TargetFloor);

        elevator.MoveToFloor(command.TargetFloor);
        return true;
    }
}
