namespace VttMvuModel.Pieces;

using VttMvuModel.Board;

public sealed record PieceLocation(string? CellId, GridCoordinate? Coordinate, double OffsetX = 0, double OffsetY = 0);
