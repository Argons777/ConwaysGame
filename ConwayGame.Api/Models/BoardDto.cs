﻿namespace ConwayGame.Api.Models;

/// <summary>
/// 
/// </summary>
public class BoardDto
{
    public Guid Id { get; set; }
    public int Generation { get; set; }
    public int Population { get; set; }
    public List<List<bool>> Cells { get; set; }
}