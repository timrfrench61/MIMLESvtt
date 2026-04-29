namespace VttMvuView.Tabletop;

public sealed record TabletopTokenState(string TokenId, string Name, int X, int Y, int RotationDegrees);

public sealed record TabletopState(
    int BoardWidth,
    int BoardHeight,
    IReadOnlyList<TabletopTokenState> Tokens,
    string? SelectedTokenId,
    int TurnNumber,
    string TurnPhase,
    string ActiveToolId,
    int? LastDiceRoll,
    string? StatusMessage);
