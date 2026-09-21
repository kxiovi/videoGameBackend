using GameStore.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Data;

public static class DataExtensions
{
    public static void MigrateDb(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider
                             .GetRequiredService<GameStoreContext>();
        dbContext.Database.Migrate();
    }

    public static void AddGameStoreDb(this WebApplicationBuilder builder)
    {
        var connString = builder.Configuration.GetConnectionString("GameStore");

        // Dbcontext has a scoped service lifetime to ensure that a new instance of 
        // Dbcontext is created per request.  
        // DB connections are limited and expensive + Dbcontext is not threadsafe.
        // Scoped avoides both. 
        // so scoped makes it easier to manage transactions and ensure data consistency.
        builder.Services.AddScoped<GameStoreContext>();
        builder.Services.AddSqlite<GameStoreContext>(
            connString, 
            optionsAction: options => options.UseSeeding((context, _) =>
            {
                if (!context.Set<Genre>().Any())
                    {
                        context.Set<Genre>().AddRange(
                            new Genre {Name = "Platformer"}, 
                            new Genre {Name = "RPG"}, 
                            new Genre {Name = "Racing"}, 
                            new Genre {Name = "Adventure"}, 
                            new Genre {Name = "Survival"}
                        );

                        context.SaveChanges();
                }
            })
        );
    }
}
