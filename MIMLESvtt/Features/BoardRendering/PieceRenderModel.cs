namespace VttMvuView.BoardRendering;
public sealed record PieceRenderModel(string PieceId, double X, double Y, double Width, double Height, double RotationDegrees, string? ImageAssetId, string CssClass);
