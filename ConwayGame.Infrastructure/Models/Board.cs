using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace ConwayGame.Infrastructure.Models;

public class Board
{
    public Guid Id { get; set; }
    public int Rows { get; set; }
    public int Columns { get; set; }
    public bool[] Cells { get; set; }
    public int Generation { get; set; }
    public int Population { get; set; }
}