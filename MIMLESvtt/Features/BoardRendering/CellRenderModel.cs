namespace VttMvuView.BoardRendering;
public sealed record CellRenderModel(string CellId, double X, double Y, double Width, double Height, string CssClass, string? Label);
