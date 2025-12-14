using System.Threading.Tasks;
using ElevatorControl.Api.Application.Commands;
using ElevatorControl.Api.Application.DTOs;
using ElevatorControl.Api.Infrastructure.Repositories;

namespace ElevatorControl.Api.Application.Handlers;

public class CreateElevatorCommandHandler
{
    private readonly ILogger<CreateElevatorCommandHandler> _logger;
    private readonly IElevatorRepository _repository;

    public CreateElevatorCommandHandler(IElevatorRepository repository, ILogger<CreateElevatorCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<CreateElevatorResultDto> HandleAsync(CreateElevatorCommand command)
    {
        var elevator = await _repository.CreateAsync();

        _logger.LogInformation("Created elevator {Id} at floor {Floor}",
            elevator.Id, elevator.CurrentFloor);

        return new CreateElevatorResultDto(elevator.Id, elevator.CurrentFloor);
    }
}
