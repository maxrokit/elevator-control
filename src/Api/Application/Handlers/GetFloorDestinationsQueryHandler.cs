using System.Linq;
using System.Threading.Tasks;
using ElevatorControl.Api.Application.Queries;
using ElevatorControl.Api.Infrastructure.Repositories;

namespace ElevatorControl.Api.Application.Handlers;

public class GetFloorDestinationsQueryHandler
{
    private readonly IElevatorRepository _repository;

    public GetFloorDestinationsQueryHandler(IElevatorRepository repository)
    {
        _repository = repository;
    }

    public async Task<int[]?> HandleAsync(GetFloorDestinationsQuery query)
    {
        var elevator = await _repository.GetByIdAsync(query.ElevatorId);
        if (elevator == null) return null;

        lock (elevator.FloorDestinations)
        {
            return elevator.FloorDestinations.ToArray();
        }
    }
}
