using ConwayGame.Infrastructure.Data;
using ConwayGame.Infrastructure.DataAccess;
using ConwayGame.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace ConwayGame.Api;

public class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration().WriteTo.Console()
                                              .CreateLogger();
        try
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<ConwayDbContext>(options => options.UseSqlite("Data Source=ConwayGame.db"));
            builder.Services.AddScoped<IBoardRepository, BoardRepository>();
            builder.Services.AddScoped<IBoardService, BoardService>();

            builder.Services.AddAutoMapper(typeof(Program).Assembly);

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Host.UseSerilog();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Unexpected exception");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}