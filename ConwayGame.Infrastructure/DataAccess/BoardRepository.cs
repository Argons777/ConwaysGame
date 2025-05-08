using ConwayGame.Infrastructure.Data;
using ConwayGame.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace ConwayGame.Infrastructure.DataAccess;

public class BoardRepository : IBoardRepository
{
    private readonly ConwayDbContext _context;

    public BoardRepository(ConwayDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Board>> GetAllAsync() => await _context.Set<Board>().ToListAsync();

    public async Task<Board?> GetByIdAsync(Guid id) => await _context.Set<Board>().FindAsync(id);

    public async Task<Guid> AddAsync(Board board)
    {
        _context.Set<Board>().Add(board);
        await _context.SaveChangesAsync();
        
        return board.Id;
    }

    public async Task UpdateAsync(Board board)
    {
        _context.Set<Board>().Update(board);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var board = await GetByIdAsync(id);
        if (board != null)
        {
            _context.Set<Board>().Remove(board);
            await _context.SaveChangesAsync();
        }
    }
}