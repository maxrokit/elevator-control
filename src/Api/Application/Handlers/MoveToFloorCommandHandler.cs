using System.Threading.Tasks;
using ElevatorControl.Api.Application.Commands;
using ElevatorControl.Api.Infrastructure.Repositories;

namespace ElevatorControl.Api.Application.Handlers;

public class MoveToFloorCommandHandler
{
    private readonly IElevatorRepository _repository;

    public MoveToFloorCommandHandler(IElevatorRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(MoveToFloorCommand command)
    {
        var elevator = await _repository.GetByIdAsync(command.ElevatorId);
        elevator?.MoveToFloor(command.TargetFloor);
    }
}
