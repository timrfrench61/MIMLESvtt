namespace VttMvuModel.Pieces;

public sealed record PieceInstance(string PieceId, string PieceDefinitionId, string DisplayName, string? OwnerSeatId, PieceLocation Location, PieceFacing Facing, bool IsSelected, bool IsHidden, IReadOnlyDictionary<string, string> Properties);
