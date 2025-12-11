using System.Threading.Tasks;
using ElevatorControl.Api.Application.Commands;
using ElevatorControl.Api.Infrastructure.Repositories;

namespace ElevatorControl.Api.Application.Handlers;

public class AddFloorCallCommandHandler
{
    private readonly IElevatorRepository _repository;

    public AddFloorCallCommandHandler(IElevatorRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> HandleAsync(AddFloorCallCommand command)
    {
        var elevator = await _repository.GetByIdAsync(command.ElevatorId);
        if (elevator == null) return false;

        elevator.AddFloorCall(command.Floor, command.Direction);
        return true;
    }
}
