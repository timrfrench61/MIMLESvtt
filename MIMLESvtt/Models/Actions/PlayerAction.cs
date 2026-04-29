namespace VttMvuModel.Actions;

public sealed record PlayerAction(string ActionId, string ActionType, string? SeatId, DateTimeOffset OccurredAt, IReadOnlyDictionary<string, string> Parameters);
