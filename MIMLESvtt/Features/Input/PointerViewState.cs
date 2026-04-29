namespace VttMvuView.Input;
public sealed record PointerViewState(double X, double Y, bool IsDown, string? HoveredItemId, PointerMode Mode);
