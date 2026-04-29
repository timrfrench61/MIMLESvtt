namespace VttMvuModel.Table;

using VttMvuModel.Board;
using VttMvuModel.Pieces;
using VttMvuModel.Markers;

public sealed record TableState(BoardState Board, IReadOnlyDictionary<string, PieceInstance> PiecesById, IReadOnlyList<TableMarker> Markers, IReadOnlyList<TableNote> Notes);
