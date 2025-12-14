using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElevatorControl.Api.Application.DTOs;
using ElevatorControl.Api.Application.Mappers;
using ElevatorControl.Api.Application.Queries;
using ElevatorControl.Api.Infrastructure.Repositories;

namespace ElevatorControl.Api.Application.Handlers;

public class GetElevatorsQueryHandler
{
    private readonly ILogger<GetElevatorsQueryHandler> _logger;
    private readonly IElevatorRepository _repository;

    public GetElevatorsQueryHandler(IElevatorRepository repository, ILogger<GetElevatorsQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IEnumerable<ElevatorDto>> HandleAsync(GetElevatorsQuery query)
    {
        var elevators = await _repository.GetAllAsync();
        var elevatorList = elevators.ToList();

        _logger.LogInformation("Retrieved {Count} elevators", elevatorList.Count);

        return elevatorList.Select(e => e.ToDto());
    }
}
