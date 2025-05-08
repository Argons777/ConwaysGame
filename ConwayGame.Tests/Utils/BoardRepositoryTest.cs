using ConwayGame.Infrastructure.DataAccess;
using ConwayGame.Infrastructure.Models;

namespace ConwayGame.Tests.Utils;

public class BoardRepositoryTest : IBoardRepository
{
    public async Task<IEnumerable<Board>> GetAllAsync() => throw new NotImplementedException();

    public async Task<Board?> GetByIdAsync(Guid id)
    {
        switch (id.ToString())
        {
            case "11111111-1111-1111-1111-111111111111":
                return new Board()
                {
                    Rows = 6,
                    Columns = 6,
                    Cells = DataGenerator.BlinkerArrayHorizontal,
                    Population = 3,
                    Generation = 1
                };
            case "22222222-2222-2222-2222-222222222222":
                return new Board()
                {
                    Rows = 6,
                    Columns = 6,
                    Cells = DataGenerator.BeaconStep1,
                    Population = 8,
                    Generation = 1
                };
            case "33333333-3333-3333-3333-333333333333":
                return new Board()
                {
                    Rows = 6,
                    Columns = 6,
                    Cells = DataGenerator.BlockStep1,
                    Population = 3,
                    Generation = 1
                };
            case "44444444-4444-4444-4444-444444444444":
                return new Board()
                {
                    Rows = 6,
                    Columns = 6,
                    Cells = DataGenerator.GliderStep1,
                    Population = 5,
                    Generation = 1
                };
        }

        return null;
    }

    public async Task<Guid> AddAsync(Board board) => Guid.NewGuid();

    public async Task UpdateAsync(Board board)
    {
    }

    public async Task DeleteAsync(Guid id)
    {
    }
}