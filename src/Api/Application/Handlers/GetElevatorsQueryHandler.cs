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
    private readonly IElevatorRepository _repository;

    public GetElevatorsQueryHandler(IElevatorRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ElevatorDto>> HandleAsync(GetElevatorsQuery query)
    {
        var elevators = await _repository.GetAllAsync();
        return elevators.Select(e => e.ToDto());
    }
}
