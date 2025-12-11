using System.Threading.Tasks;
using ElevatorControl.Api.Application.Commands;
using ElevatorControl.Api.Application.DTOs;
using ElevatorControl.Api.Infrastructure.Repositories;

namespace ElevatorControl.Api.Application.Handlers;

public class CreateElevatorCommandHandler
{
    private readonly IElevatorRepository _repository;

    public CreateElevatorCommandHandler(IElevatorRepository repository)
    {
        _repository = repository;
    }

    public async Task<CreateElevatorResultDto> HandleAsync(CreateElevatorCommand command)
    {
        var elevator = await _repository.CreateAsync();
        return new CreateElevatorResultDto(elevator.Id, elevator.CurrentFloor);
    }
}
