using System.Threading.Tasks;
using System.Threading.Tasks;
using ElevatorControl.Api.Application.Queries;
using ElevatorControl.Api.Infrastructure.Repositories;

namespace ElevatorControl.Api.Application.Handlers;

public class GetNextFloorQueryHandler
{
    private readonly IElevatorRepository _repository;

    public GetNextFloorQueryHandler(IElevatorRepository repository)
    {
        _repository = repository;
    }

    public async Task<int?> HandleAsync(GetNextFloorQuery query)
    {
        var elevator = await _repository.GetByIdAsync(query.ElevatorId);
        return elevator?.GetNextFloor();
    }
}
