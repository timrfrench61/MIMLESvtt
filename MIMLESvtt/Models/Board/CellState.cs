namespace VttMvuModel.Board;

public sealed record CellState(string CellId, GridCoordinate Coordinate, string? TerrainId, bool IsBlocked);
