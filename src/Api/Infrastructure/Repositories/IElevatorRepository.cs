using ElevatorControl.Api.Domain.Entities;

namespace ElevatorControl.Api.Infrastructure.Repositories;

public interface IElevatorRepository
{
    Task<Elevator?> GetByIdAsync(int id);
    Task<IEnumerable<Elevator>> GetAllAsync();
    Task<Elevator> CreateAsync();
}
