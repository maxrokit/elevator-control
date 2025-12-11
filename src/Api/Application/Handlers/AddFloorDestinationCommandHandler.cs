using System.Threading.Tasks;
using ElevatorControl.Api.Application.Commands;
using ElevatorControl.Api.Infrastructure.Repositories;

namespace ElevatorControl.Api.Application.Handlers;

public class AddFloorDestinationCommandHandler
{
    private readonly IElevatorRepository _repository;

    public AddFloorDestinationCommandHandler(IElevatorRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> HandleAsync(AddFloorDestinationCommand command)
    {
        var elevator = await _repository.GetByIdAsync(command.ElevatorId);
        if (elevator == null) return false;

        elevator.AddFloorDestination(command.Floor);
        return true;
    }
}
