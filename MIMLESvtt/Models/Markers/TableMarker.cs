namespace VttMvuModel.Markers;

using VttMvuModel.Pieces;

public sealed record TableMarker(string MarkerId, string Label, string MarkerType, PieceLocation Location, bool IsVisibleToPlayers);
