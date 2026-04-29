namespace VttMvuView.BoardRendering;
public sealed record BoardRenderModel(BoardRenderBounds Bounds, IReadOnlyList<CellRenderModel> Cells, IReadOnlyList<PieceRenderModel> Pieces, IReadOnlyList<OverlayRenderModel> Overlays);
