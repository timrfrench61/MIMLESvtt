namespace VttMvuView.Tabletop;
public sealed record BoardViewportViewState(double Zoom, double PanX, double PanY, bool ShowGrid, bool ShowCoordinates, ViewportMode Mode);
