namespace GameStore.Api.Dtos;

// has genreID (int) instead of genre (string)

public record GameDetailsDto(
    int Id,
    string Name, 
    int GenreId, 
    decimal Price, 
    DateOnly ReleaseDate
);