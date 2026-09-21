using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints;

// extension classes always live in static classes
public static class GamesEndpoints
{
    const string GetGameEndpointName = "GetGame";
    // M in 19.99M tells compiler that it's meant to use the '.' as a decimal
    private static readonly List<GameSummaryDto> games = [
        new (
            1, 
            "Street Fighter II",
            "Fighting", 
            19.99M, 
            new DateOnly(1992, 7, 15)
        ), 
        new (
            2, 
            "Mario Bros",
            "Platform", 
            59.99M, 
            new DateOnly(1990, 10, 15)
        ), 
        new (
            3, 
            "Counterstrike",
            "FPS", 
            20.00M, 
            new DateOnly(2004, 7, 12)
        )
    ];

    // extend WebApplication 
    public static void MapGamesEndpoint(this WebApplication app)
    {
        // now can replace app. with group.
        var group = app.MapGroup("/games");

        // GET endpoint that returns all the games available in API i.e. in /games
        group.MapGet("/", async (GameStoreContext dbContext) 
            => await dbContext.Games
                              .Select(game=> new GameSummaryDto(
                                game.Id,
                                game.Name,
                                game.Genre!.Name,  // using the ! to tell compiler that Genre does have a Name property
                                game.Price,
                                game.ReleaseDate
                              ))
                              .AsNoTracking()
                              .ToListAsync());

        // GET a single game and not all games
        group.MapGet("/{id}", async (int id, GameStoreContext dbContext) => 
        {
            var game = await dbContext.Games.FindAsync(id);
            return game is null ? Results.NotFound() : Results.Ok(
                new GameDetailsDto(
                    game.Id,
                    game.Name,
                    game.GenreId,
                    game.Price,
                    game.ReleaseDate
                )
            );

        })
        .WithName(GetGameEndpointName)
        ;

        // POST endpoint that targets /games
        // to add a new game
        // thus we need a new dto to represent the incoming payload for the new game we want to create
        // CreatedAtRoute generates a Location that can used by clients to head over to to reach the new game that was created
        group.MapPost("/", async (CreateGameDto newGame, GameStoreContext dbContext) =>
        {
            Game game = new()
            {
              Name = newGame.Name, 
              GenreId = newGame.GenreId, 
              Price = newGame.Price, 
              ReleaseDate = newGame.ReleaseDate  
            };

            // the entitycore will keep track of the game; it hasn't been added to DB yet
            dbContext.Games.Add(game);

            // add to DB
            await dbContext.SaveChangesAsync();

            GameDetailsDto gameDto = new(
                game.Id, 
                game.Name, 
                game.GenreId, 
                game.Price,
                game.ReleaseDate
            );

            return Results.CreatedAtRoute(GetGameEndpointName, new {id = gameDto}, gameDto); 
        });


        // PUT endpoint that targets /games/Id
        // update an existing game
        // must define the shape of the input payload
        group.MapPut("/{id}", async (int id, 
                                UpdateGameDto updatedGame,
                                GameStoreContext dbContext) =>
        {
            var existingGame = await dbContext.Games.FindAsync(id);
            if (existingGame is null)
            {
                return Results.NotFound();
            }
            existingGame.Name = updatedGame.Name;
            existingGame.GenreId = updatedGame.GenreId;
            existingGame.Price = updatedGame.Price;
            existingGame.ReleaseDate = updatedGame.ReleaseDate;

            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        });

        // DELETE /games/1
        // just recieves ID of the game to delete
        // no need to check even if the specified game id exists
        group.MapDelete("/{id}", async (int id, GameStoreContext dbContext) =>
        {
            await dbContext.Games
                           .Where(game => game.Id == id)
                           .ExecuteDeleteAsync();
            return Results.NoContent();
        });
    }
}
