using ConwayGame.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace ConwayGame.Infrastructure.Data;

public class ConwayDbContext : DbContext
{
    public DbSet<Board> Boards { get; set; }

    public ConwayDbContext(DbContextOptions<ConwayDbContext> options) : base(options)
    {
    }

    public ConwayDbContext()
    {
    }
}