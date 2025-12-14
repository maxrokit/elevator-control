using System.Threading.Tasks;
using ElevatorControl.Api.Application.Commands;
using ElevatorControl.Api.Infrastructure.Repositories;

namespace ElevatorControl.Api.Application.Handlers;

public class AddFloorDestinationCommandHandler
{
    private readonly ILogger<AddFloorDestinationCommandHandler> _logger;
    private readonly IElevatorRepository _repository;

    public AddFloorDestinationCommandHandler(IElevatorRepository repository, ILogger<AddFloorDestinationCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<bool> HandleAsync(AddFloorDestinationCommand command)
    {
        var elevator = await _repository.GetByIdAsync(command.ElevatorId);
        if (elevator == null) return false;

        elevator.AddFloorDestination(command.Floor);

        _logger.LogInformation("Added floor destination {Floor} to elevator {Id}",
            command.Floor, command.ElevatorId);

        return true;
    }
}
