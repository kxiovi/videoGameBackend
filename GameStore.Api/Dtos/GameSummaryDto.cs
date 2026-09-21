namespace GameStore.Api.Dtos;

// A DTO (Data Transfer Object) is a contract b/w the client and server
// it's an agreement about how the data will be transferred and used

public record GameSummaryDto(
    int Id,
    string Name, 
    string Genre, 
    decimal Price, 
    DateOnly ReleaseDate
);