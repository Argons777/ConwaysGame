using ConwayGame.Infrastructure.DataAccess;
using ConwayGame.Infrastructure.Models;
using ConwayGame.Infrastructure.Services;
using ConwayGame.Tests.Utils;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace ConwayGame.Tests.Services;

public class BoardServiceTests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public BoardServiceTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    #region GenerateRandomBoardAsync

    [Theory]
    [InlineData(3, 3)]
    [InlineData(3, 4)]
    [InlineData(5, 3)]
    [InlineData(50, 50)]
    public async Task GenerateRandomBoardAsync_CorrectSize_BoardCreated(int rows, int cols)
    {
        // Arrange
        var repositoryMock = new Mock<IBoardRepository>();
        var loggerMock = new Mock<ILogger<BoardService>>();
        var boardService = new BoardService(repositoryMock.Object, loggerMock.Object);

        // Act
        var result = await boardService.GenerateRandomBoardAsync(rows, cols);

        // Assert
        var totalCells = rows * cols;

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(rows, result.Value.Rows);
        Assert.Equal(cols, result.Value.Columns);
        Assert.Equal(totalCells, result.Value.Cells.Length);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(3, 1)]
    [InlineData(2, 100)]
    [InlineData(60, -1)]
    public async Task GenerateRandomBoardAsync_IncorrectSize_OperationFailed(int rows, int cols)
    {
        // Arrange
        var repositoryMock = new Mock<IBoardRepository>();
        var loggerMock = new Mock<ILogger<BoardService>>();
        var boardService = new BoardService(repositoryMock.Object, loggerMock.Object);

        // Act
        var result = await boardService.GenerateRandomBoardAsync(rows, cols);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Value);
        Assert.Equal("The number of rows and cols must be greater than 3", result.ErrorMessage);
    }

    [Theory]
    [InlineData(3, 3)]
    [InlineData(100, 100)]
    public async Task GenerateRandomBoardAsync_RepositoryError_BoardNotSaved(int rows, int cols)
    {
        // Arrange
        var repositoryMock = new Mock<IBoardRepository>();
        repositoryMock.Setup(x => x.AddAsync(It.IsAny<Board>()))
                      .ThrowsAsync(new Exception());
        var loggerMock = new Mock<ILogger<BoardService>>();
        var boardService = new BoardService(repositoryMock.Object, loggerMock.Object);

        // Act
        var result = await boardService.GenerateRandomBoardAsync(rows, cols);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Value);
        Assert.Equal("Unexpected error creating the new board.", result.ErrorMessage);
    }

    #endregion

    #region CreateBoardFromMatrixAsync

    [Theory]
    [InlineData(3, 3)]
    [InlineData(10, 10)]
    [InlineData(8, 16)]
    [InlineData(50, 6)]
    public async Task CreateBoardFromMatrixAsync_CorrectMatrix_BoardCreated(int rows, int cols)
    {
        // Arrange
        var repositoryMock = new Mock<IBoardRepository>();
        var loggerMock = new Mock<ILogger<BoardService>>();
        var boardService = new BoardService(repositoryMock.Object, loggerMock.Object);
        var matrix = DataGenerator.GenerateMatrix(rows, cols);

        // Act
        var result = await boardService.CreateBoardFromMatrixAsync(matrix);

        // Assert
        var totalCells = rows * cols;

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(rows, result.Value.Rows);
        Assert.Equal(cols, result.Value.Columns);
        Assert.Equal(totalCells, result.Value.Cells.Length);
        Assert.Equal(matrix.SelectMany(row => row).ToArray(), result.Value.Cells);
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(3, 1)]
    [InlineData(2, 100)]
    [InlineData(60, -1)]
    public async Task CreateBoardFromMatrixAsync_IncorrectSize_OperationFailed(int rows, int cols)
    {
        // Arrange
        var repositoryMock = new Mock<IBoardRepository>();
        var loggerMock = new Mock<ILogger<BoardService>>();
        var boardService = new BoardService(repositoryMock.Object, loggerMock.Object);
        var matrix = DataGenerator.GenerateMatrix(rows, cols);

        // Act
        var result = await boardService.CreateBoardFromMatrixAsync(matrix);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Value);
        Assert.Equal("The number of rows and cols must be greater than 3", result.ErrorMessage);
    }

    [Theory]
    [MemberData(nameof(DataGenerator.IncorrectMatrixElementCount), MemberType = typeof(DataGenerator))]
    public async Task CreateBoardFromMatrixAsync_IncorrectNumberElements_OperationFailed(List<List<bool>> matrix)
    {
        // Arrange
        var repositoryMock = new Mock<IBoardRepository>();
        var loggerMock = new Mock<ILogger<BoardService>>();
        var boardService = new BoardService(repositoryMock.Object, loggerMock.Object);

        // Act
        var result = await boardService.CreateBoardFromMatrixAsync(matrix);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Value);
        Assert.Equal("All rows must have the same number of elements", result.ErrorMessage);
    }

    #endregion

    #region GenerateNextBoardStateAsync

    [Theory]
    [InlineData("11111111-1111-1111-1111-111111111111", 3, 2)]
    [InlineData("22222222-2222-2222-2222-222222222222", 6, 2)]
    public async Task GenerateNextBoardStateAsync_KnownPatterns_StateGenerated(Guid id, int population, int generation)
    {
        // Arrange
        bool[] nextState = null;
        if (id.Equals(new Guid("11111111-1111-1111-1111-111111111111")))
        {
            nextState = DataGenerator.BlinkerArrayVertical;
        }
        else if (id.Equals(new Guid("22222222-2222-2222-2222-222222222222")))
        {
            nextState = DataGenerator.BeaconStep2;
        }

        var repositoryMock = new BoardRepositoryTest();
        var loggerMock = new Mock<ILogger<BoardService>>();
        var boardService = new BoardService(repositoryMock, loggerMock.Object);

        // Act
        var result = await boardService.GenerateNextBoardStateAsync(id);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(generation, result.Value.Generation);
        Assert.Equal(population, result.Value.Population);
        Assert.True(result.Value.Cells.SequenceEqual(nextState!));
    }

    #endregion

    #region GenerateFinalBoardStateAsync

    [Theory]
    [InlineData("33333333-3333-3333-3333-333333333333", 4, 3)]
    public async Task GenerateFinalBoardStateAsync_BlockPattern_FinalStateFound(Guid id, int population, int generation)
    {
        // Arrange
        var repositoryMock = new BoardRepositoryTest();
        var loggerMock = new Mock<ILogger<BoardService>>();
        var boardService = new BoardService(repositoryMock, loggerMock.Object);

        // Act
        var result = await boardService.GenerateFinalBoardStateAsync(id);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(generation, result.Value.Generation);
        Assert.Equal(population, result.Value.Population);
        Assert.True(result.Value.Cells.SequenceEqual(DataGenerator.BlockStep2));
    }

    [Theory]
    [InlineData("11111111-1111-1111-1111-111111111111")]
    [InlineData("22222222-2222-2222-2222-222222222222")]
    public async Task GenerateFinalBoardStateAsync_RepeatingPatterns_FinalStateNotFound(Guid id)
    {
        // Arrange
        var repositoryMock = new BoardRepositoryTest();
        var loggerMock = new Mock<ILogger<BoardService>>();
        var boardService = new BoardService(repositoryMock, loggerMock.Object);

        // Act
        var result = await boardService.GenerateFinalBoardStateAsync(id);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Null(result.Value);
        Assert.Equal("Final state not achieved after 10 attempts", result.ErrorMessage);
    }

    #endregion

    #region GenerateXBoardStatesAsync

    [Theory]
    [InlineData("44444444-4444-4444-4444-444444444444", 5, 4)]
    public async Task GenerateXBoardStatesAsync_RepeatingPatterns_StateGenerated(Guid id, int population, int delta)
    {
        // Arrange
        var nextState = delta switch
                        {
                            1 => DataGenerator.GliderStep2,
                            2 => DataGenerator.GliderStep3,
                            3 => DataGenerator.GliderStep4,
                            4 => DataGenerator.GliderStep5,
                            _ => null
                        };
        var repositoryMock = new BoardRepositoryTest();
        var loggerMock = new Mock<ILogger<BoardService>>();
        var boardService = new BoardService(repositoryMock, loggerMock.Object);

        // Act
        var result = await boardService.GenerateXBoardStatesAsync(id, delta);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(population, result.Value.Population);
        Assert.True(result.Value.Cells.SequenceEqual(nextState!));
    }

    #endregion
}