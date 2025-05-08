using ConwayGame.Infrastructure.DataAccess;
using ConwayGame.Infrastructure.Models;
using ConwayGame.Infrastructure.Utils;
using Microsoft.Extensions.Logging;

namespace ConwayGame.Infrastructure.Services;

public class BoardService : IBoardService
{
    private const bool ALIVE = true;
    private const bool DEAD = false;

    // Holds the locations of the 8 neighbors surrounding the current cell, which is at the center
    private static readonly int[][] NeighborsLocations =
    [
        [-1, -1], // ↖️
        [-1, 0], // ⬆️
        [-1, 1], // ↗️
        [0, -1], // ⬅️
        [0, 1], // ➡️
        [1, -1], // ↙️
        [1, 0], // ⬇️
        [1, 1] // ↘️
    ];

    private readonly IBoardRepository _repository;
    private readonly ILogger<BoardService> _logger;

    public BoardService(IBoardRepository repository, ILogger<BoardService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    #region " Repository calls "

    public async Task<Board?> GetBoardByIdAsync(Guid id) => await _repository.GetByIdAsync(id);

    #endregion

    #region " Service calls "

    public async Task<OperationResult<Board?>> CreateBoardFromMatrixAsync(List<List<bool>> matrix)
    {
        try
        {
            var rows = matrix.Count;
            var cols = rows > 0 ? matrix[0].Count : 0;
            if (rows < 3 || cols < 3)
                return OperationResult<Board>.Failure("The number of rows and cols must be greater than 3");

            var newArray = matrix.SelectMany(row => row).ToArray();
            if (newArray.Length != rows * cols)
                return OperationResult<Board>.Failure("All rows must have the same number of elements");

            var newBoard = new Board
            {
                Id = Guid.NewGuid(),
                Rows = rows,
                Columns = cols,
                Cells = newArray,
                Generation = 1,
                Population = newArray.Count(cell => cell is ALIVE)
            };

            await _repository.AddAsync(newBoard);

            return OperationResult<Board?>.Success(newBoard);
        }
        catch (InvalidCastException ex)
        {
            _logger.LogError(ex, "Error creating the new board: {@Matrix}", matrix);
            return OperationResult<Board?>.Failure("All the values in the matrix must be between 0 and 1.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating the new board: {@Matrix}", matrix);
            return OperationResult<Board?>.Failure("Unexpected error creating the new board.");
        }
    }

    public async Task<OperationResult<Board?>> GenerateRandomBoardAsync(int rows, int cols)
    {
        try
        {
            if (rows < 3 || cols < 3)
                return OperationResult<Board>.Failure("The number of rows and cols must be greater than 3");

            var random = new Random();

            var newBoard = new Board
            {
                Id = Guid.NewGuid(),
                Rows = rows,
                Columns = cols,
                Generation = 1,
                Cells = new bool[rows * cols]
            };

            for (var index = 0; index < rows * cols; index++)
            {
                var isAlive = random.Next(2) == 1;
                newBoard.Cells[index] = isAlive;
                newBoard.Population += isAlive ? 1 : 0;
            }

            await _repository.AddAsync(newBoard);

            return OperationResult<Board?>.Success(newBoard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating the new board");
            return OperationResult<Board?>.Failure("Unexpected error creating the new board.");
        }
    }

    public async Task<OperationResult<Board?>> GenerateNextBoardStateAsync(Guid id, bool saveDb = true)
    {
        try
        {
            var board = await _repository.GetByIdAsync(id);
            if (board is null) return OperationResult<Board?>.Failure("Board not found");

            return await GenerateNextBoardStateInternalAsync(board, saveDb);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating next state. BoardId: {BoardId}", id);
            return OperationResult<Board?>.Failure("Error generating next state");
        }
    }

    public async Task<OperationResult<Board?>> GenerateXBoardStatesAsync(Guid id, int statesToGenerate = 10)
    {
        var statesCounter = 0;
        try
        {
            if (statesToGenerate <= 0)
                return OperationResult<Board?>.Failure(
                    "The number of board states to generate must be greater than zero.");

            var board = await _repository.GetByIdAsync(id);
            if (board is null) return OperationResult<Board?>.Failure("Board not found");

            OperationResult<Board?> result = null;
            while (statesCounter < statesToGenerate)
            {
                result = await GenerateNextBoardStateInternalAsync(board, false);

                if (!result.IsSuccess)
                    return OperationResult<Board?>.Failure($"Error generating state {statesCounter}");

                statesCounter++;
            }

            await _repository.UpdateAsync(result!.Value!);

            return OperationResult<Board?>.Success(result.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating Xth state");
            return OperationResult<Board?>.Failure("Error generating Xth state");
        }
    }

    public async Task<OperationResult<Board?>> GenerateFinalBoardStateAsync(Guid id, int attempts = 10)
    {
        try
        {
            if (attempts is 0) return OperationResult<Board?>.Failure("Number of attempts must be greater than zero.");

            var board = await _repository.GetByIdAsync(id);
            if (board is null) return OperationResult<Board?>.Failure("Board not found");

            var isFinalState = false;
            var attemptsCounter = 0;
            OperationResult<Board?> result = null;

            while (attemptsCounter < attempts && !isFinalState)
            {
                var previousState = board.Cells.ToArray();
                result = await GenerateNextBoardStateInternalAsync(board, false);

                if (!result.IsSuccess)
                    return OperationResult<Board?>.Failure($"Error generating state {attemptsCounter}");

                if (previousState.SequenceEqual(board.Cells)) isFinalState = true;

                attemptsCounter++;
            }

            if (isFinalState)
            {
                await _repository.UpdateAsync(result!.Value!);
                return OperationResult<Board?>.Success(result.Value!);
            }

            return OperationResult<Board?>.Failure($"Final state not achieved after {attempts} attempts");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating final state");
            return OperationResult<Board?>.Failure("Error generating final state");
        }
    }

    #endregion

    #region " Private methods "

    private int CountAliveNeighbors(Board board, bool[] originalBoard, int currentIndex)
    {
        var aliveNeighbors = 0;
        var currentRow = currentIndex / board.Columns;
        var currentCol = currentIndex % board.Columns;

        foreach (var neighbor in NeighborsLocations)
        {
            var neighborRow = currentRow + neighbor[0];
            var neighborCol = currentCol + neighbor[1];

            if (neighborRow >= 0 && neighborRow < board.Rows &&
                neighborCol >= 0 && neighborCol < board.Columns)
            {
                var neighborIndex = neighborRow * board.Columns + neighborCol;
                if (originalBoard[neighborIndex] is ALIVE) aliveNeighbors++;
            }
        }

        return aliveNeighbors;
    }

    private async Task<OperationResult<Board?>> GenerateNextBoardStateInternalAsync(Board board, bool saveDb = true)
    {
        try
        {
            var originalBoard = board.Cells.ToArray();
            for (var currentIndex = 0; currentIndex < board.Cells.Length; currentIndex++)
            {
                var aliveNeighbors = CountAliveNeighbors(board, originalBoard, currentIndex);

                board.Cells[currentIndex] = originalBoard[currentIndex] switch
                                            {
                                                ALIVE when aliveNeighbors < 2 => DEAD, // Underpopulation
                                                ALIVE when aliveNeighbors > 3 => DEAD, // Overpopulation
                                                DEAD when aliveNeighbors is 3 => ALIVE, // Revives
                                                _ => originalBoard[currentIndex]
                                            };
            }

            board.Generation++;
            board.Population = board.Cells.Count(cell => cell is ALIVE);

            if (saveDb) await _repository.UpdateAsync(board);

            return OperationResult<Board?>.Success(board);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating next state. BoardId: {BoardId}", board.Id);
            return OperationResult<Board?>.Failure("Error generating next state");
        }
    }

    #endregion
}