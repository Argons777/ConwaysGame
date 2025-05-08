using ConwayGame.Infrastructure.Models;

namespace ConwayGame.Infrastructure.DataAccess;

public interface IBoardRepository
{
    Task<IEnumerable<Board>> GetAllAsync();
    Task<Board?> GetByIdAsync(Guid id);
    Task<Guid> AddAsync(Board board);
    Task UpdateAsync(Board board);
    Task DeleteAsync(Guid id);
}