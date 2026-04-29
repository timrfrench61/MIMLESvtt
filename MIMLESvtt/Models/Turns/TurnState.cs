namespace VttMvuModel.Turns;

public sealed record TurnState(int TurnNumber, string? ActiveSeatId, TurnPhase Phase, IReadOnlyList<string> TurnOrderSeatIds);
