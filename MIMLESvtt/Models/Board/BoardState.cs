namespace VttMvuModel.Board;

public sealed record BoardState(BoardDefinition Definition, IReadOnlyDictionary<string, CellState> CellsById);
