using AutoMapper;
using ConwayGame.Api.Models;
using ConwayGame.Infrastructure.Models;

namespace ConwayGame.Api.Configuration;

public class BoardProfile : Profile
{
    public BoardProfile()
    {
        CreateMap<Board, BoardDto>()
            .ForMember(dest => dest.Cells,
                opt => opt.MapFrom(board => ConvertArrayToMatrix(board.Cells, board.Rows, board.Columns)));
    }

    private List<List<bool>> ConvertArrayToMatrix(bool[] cellsBoard, int rows, int cols)
    {
        var matrix = new List<List<bool>>();

        for (var i = 0; i < rows; i++)
        {
            var row = new List<bool>();
            for (var j = 0; j < cols; j++)
            {
                row.Add(cellsBoard[i * cols + j]);
            }
            matrix.Add(row);
        }

        return matrix;
    }
}