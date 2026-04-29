namespace VttMvuView.BoardRendering;
public sealed record OverlayRenderModel(string OverlayId, OverlayKind Kind, double X, double Y, string CssClass, string? Text);
