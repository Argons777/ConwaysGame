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

    [HttpPost("random/{rows:int}/{cols:int}")]
    public async Task<ActionResult> CreateRandomBoard(int rows, int cols)
    {
        var result = await _boardService.GenerateRandomBoardAsync(rows, cols);
        if (!result.IsSuccess) return BadRequest(result.ErrorMessage);
        
        var dto = _mapper.Map<BoardDto>(result.Value);
        return CreatedAtAction(nameof(GetBoard), new { id = dto.Id }, dto);
    }

    [HttpPost]
    public async Task<ActionResult<BoardDto>> CreateBoard([FromBody] List<List<bool>> matrix)
    {
        var result = await _boardService.CreateBoardFromMatrixAsync(matrix);
        if (!result.IsSuccess) return BadRequest(result.ErrorMessage);

        var dto = _mapper.Map<BoardDto>(result.Value);
        return CreatedAtAction(nameof(GetBoard), new { id = dto.Id }, dto);
    }

    [HttpGet("nextState/{id:guid}")]
    public async Task<ActionResult<BoardDto>> GetBoardNextState(Guid id)
    {
        var result = await _boardService.GenerateNextBoardStateAsync(id);
        if (!result.IsSuccess) return BadRequest(result.ErrorMessage);
        
        var dto = _mapper.Map<BoardDto>(result.Value);
        return Ok(dto);
    }
    
    [HttpGet("finalState/{id:guid}/attempts/{attempts:int}")]
    public async Task<ActionResult<BoardDto>> GetBoardFinalState(Guid id, int attempts)
    {
        var result = await _boardService.GenerateFinalBoardStateAsync(id, attempts);
        if (!result.IsSuccess) return BadRequest(result.ErrorMessage);
        
        var dto = _mapper.Map<BoardDto>(result.Value);
        return Ok(dto);
    }
    
    [HttpGet("deltaState/{id:guid}/delta/{delta:int}")]
    public async Task<ActionResult<BoardDto>> GetBoardXthState(Guid id, int delta)
    {
        var result = await _boardService.GenerateXBoardStatesAsync(id, delta);
        if (!result.IsSuccess) return BadRequest(result.ErrorMessage);
        
        var dto = _mapper.Map<BoardDto>(result.Value);
        return Ok(dto);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<BoardDto>> GetBoard(Guid id)
    {
        var board = await _boardService.GetBoardByIdAsync(id);
        return board == null ? NotFound() : Ok(board);
    }

}