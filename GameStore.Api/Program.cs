// Define the code needed to bootstrap the application
using GameStore.Api.Data;
using GameStore.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// to add input validation 
// adding [Required] before an attribute is not enough without this line
builder.Services.AddValidation();

builder.AddGameStoreDb();

var app = builder.Build();

// When a request comes into the root of the application, reply with "Hello World!"
// app.MapGet("/", () => "Hello World!");

app.MapGamesEndpoint();
app.MapGenresEndpoint();

app.MigrateDb();

app.Run();
