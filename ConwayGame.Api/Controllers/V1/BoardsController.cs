using Asp.Versioning;
using AutoMapper;
using ConwayGame.Api.Models;
using ConwayGame.Infrastructure.Models;
using ConwayGame.Infrastructure.Services;
using ConwayGame.Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc;

namespace ConwayGame.Api.Controllers.V1;

/// <summary>
/// ApiController for Conway's Game of Life board management 
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/boards")]
[ApiVersion("1.0")]
public class BoardsController : ControllerBase
{
    private readonly IBoardService _boardService;
    private readonly IMapper _mapper;

    /// <inheritdoc />
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
    [HttpPost("random")]
    [ProducesResponseType(typeof(BoardDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BoardDto>> CreateRandomBoard([FromQuery] int rows = 3, [FromQuery] int cols = 3)
    {
        var result = await _boardService.GenerateRandomBoardAsync(rows, cols);
        if (!result.IsSuccess) return BadRequest(result.ErrorMessage);

        var dto = _mapper.Map<BoardDto>(result.Value);
        return CreatedAtAction(nameof(GetBoard), new { id = dto.Id }, dto);
    }

    /// <summary>
    /// Creates a new board based on the matrix send in the body
    /// </summary>
    /// <param name="matrix">Bidimensional matrix with boolean values representing the cell values</param>
    /// <returns></returns>
    /// <code>
    /// [
    ///     [true, false, true, false], 
    ///     [false, true, false, true], 
    ///     [true, false, true, false], 
    ///     [false, true, false, true]
    /// ]
    /// </code>
    [HttpPost]
    [ProducesResponseType(typeof(BoardDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BoardDto>> CreateBoard([FromBody] List<List<bool>> matrix)
    {
        var result = await _boardService.CreateBoardFromMatrixAsync(matrix);
        if (!result.IsSuccess) return BadRequest(result.ErrorMessage);

        var dto = _mapper.Map<BoardDto>(result.Value);
        return CreatedAtAction(nameof(GetBoard), new { id = dto.Id }, dto);
    }

    /// <summary>
    /// Generates the next state of a board given an id and save it to the database
    /// </summary>
    /// <param name="id">Id of the board to be updated</param>
    /// <param name="generations">Number of states to generate. If not passed then it will generate the next state</param>
    /// <returns>
    /// 200 : Board with the next state given by "generations"
    /// 400 : Error generating the next state 
    /// </returns>
    [HttpPut("{id:guid}/next")]
    [ProducesResponseType(typeof(BoardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BoardDto>> GetBoardNextState(Guid id, [FromQuery] int generations = 1)
    {
        OperationResult<Board?> result;
        if (generations is 1)
        {
            result = await _boardService.GenerateNextBoardStateAsync(id);
        }
        else
        {
            result = await _boardService.GenerateXBoardStatesAsync(id, generations);
        }

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
    [HttpPut("{id:guid}/solve")]
    [ProducesResponseType(typeof(BoardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BoardDto>> GetBoardFinalState(Guid id, [FromQuery] int attempts = 10)
    {
        var result = await _boardService.GenerateFinalBoardStateAsync(id, attempts);
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