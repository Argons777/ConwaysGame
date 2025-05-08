using AutoMapper;
using ConwayGame.Api.Models;
using ConwayGame.Infrastructure.Models;
using ConwayGame.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace ConwayGame.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class BoardsController : ControllerBase
{
    private readonly IBoardService _boardService;
    private readonly IMapper _mapper;

    public BoardsController(IBoardService boardService, IMapper mapper)
    {
        _boardService = boardService;
        _mapper = mapper;
    }

    /// <summary>
    /// Creates a new board with random cell values. Rows and columns must be greater than 3 
    /// </summary>
    /// <param name="rows">Number of rows in new board</param>
    /// <param name="cols">Number of columns in new board</param>
    /// <returns>
    /// 201 : Board created successfully
    /// 400 : Error in creation. Most likely row or col number
    /// </returns>
    [HttpPost("random/{rows:int}/{cols:int}")]
    [ProducesResponseType(typeof(BoardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BoardDto>> CreateRandomBoard(int rows, int cols)
    {
        var result = await _boardService.GenerateRandomBoardAsync(rows, cols);
        if (!result.IsSuccess) return BadRequest(result.ErrorMessage);

        var dto = _mapper.Map<BoardDto>(result.Value);
        return Ok(dto);
    }

    /// <summary>
    /// Creates a new board based on the matrix send in the body
    /// </summary>
    /// <param name="matrix">Bidimensional matrix with boolean values representing the cell values</param>
    /// <returns></returns>
    /// <example>
    /// [
    ///     [true, false, true, false], 
    ///     [false, true, false, true], 
    ///     [true, false, true, false], 
    ///     [false, true, false, true]
    /// ]
    /// </example>
    [HttpPost]
    [ProducesResponseType(typeof(BoardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BoardDto>> CreateBoard([FromBody] List<List<bool>> matrix)
    {
        var result = await _boardService.CreateBoardFromMatrixAsync(matrix);
        if (!result.IsSuccess) return BadRequest(result.ErrorMessage);

        var dto = _mapper.Map<BoardDto>(result.Value);
        return Ok(dto);
    }

    /// <summary>
    /// Gets the next state of a board given an id
    /// </summary>
    /// <param name="id">Id of the board to be updated</param>
    /// <returns></returns>
    [HttpGet("nextState/{id:guid}")]
    [ProducesResponseType(typeof(BoardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BoardDto>> GetBoardNextState(Guid id)
    {
        var result = await _boardService.GenerateNextBoardStateAsync(id);
        if (!result.IsSuccess) return BadRequest(result.ErrorMessage);

        var dto = _mapper.Map<BoardDto>(result.Value);
        return Ok(dto);
    }

    /// <summary>
    /// Tries to get the final state of a given board. Final state means no more changes are being done after a number of generations
    /// </summary>
    /// <param name="id">Id of the board to be updated</param>
    /// <param name="attempts">Number of attempts to try to find the final state</param>
    /// <returns>
    /// 200 : A final state was found
    /// 400 : No final state found after the number of attempts
    /// </returns>
    [HttpGet("finalState/{id:guid}/attempts/{attempts:int}")]
    [ProducesResponseType(typeof(BoardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BoardDto>> GetBoardFinalState(Guid id, int attempts)
    {
        var result = await _boardService.GenerateFinalBoardStateAsync(id, attempts);
        if (!result.IsSuccess) return BadRequest(result.ErrorMessage);

        var dto = _mapper.Map<BoardDto>(result.Value);
        return Ok(dto);
    }

    /// <summary>
    /// Advance x amount of generations of a given board
    /// </summary>
    /// <param name="id">Id of the board to be updated</param>
    /// <param name="delta">Number of generations to be created</param>
    /// <returns></returns>
    [HttpGet("deltaState/{id:guid}/delta/{delta:int}")]
    [ProducesResponseType(typeof(BoardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BoardDto>> GetBoardXthState(Guid id, int delta)
    {
        var result = await _boardService.GenerateXBoardStatesAsync(id, delta);
        if (!result.IsSuccess) return BadRequest(result.ErrorMessage);

        var dto = _mapper.Map<BoardDto>(result.Value);
        return Ok(dto);
    }

    /// <summary>
    /// Looks for a given board
    /// </summary>
    /// <param name="id">Id of the board to be searched</param>
    /// <returns></returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BoardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BoardDto>> GetBoard(Guid id)
    {
        var board = await _boardService.GetBoardByIdAsync(id);
        return board == null ? NotFound() : Ok(_mapper.Map<BoardDto>(board));
    }
}