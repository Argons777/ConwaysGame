using ConwayGame.Infrastructure.Models;
using ConwayGame.Infrastructure.Utils;

namespace ConwayGame.Infrastructure.Services;

public interface IBoardService
{
    #region " Repository calls "

    Task<Board?> GetBoardByIdAsync(Guid id);
    
    #endregion

    #region " Business Logic methods"

    Task<OperationResult<Board?>> CreateBoardFromMatrixAsync(List<List<bool>> matrix);

    Task<OperationResult<Board?>> GenerateRandomBoardAsync(int rows, int cols);

    Task<OperationResult<Board?>> GenerateNextBoardStateAsync(Guid id, bool saveDb = true);

    Task<OperationResult<Board?>> GenerateXBoardStatesAsync(Guid id, int statesToGenerate = 10);

    Task<OperationResult<Board?>> GenerateFinalBoardStateAsync(Guid id, int attempts = 10);

    #endregion
}