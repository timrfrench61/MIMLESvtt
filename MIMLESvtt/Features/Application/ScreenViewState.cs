namespace VttMvuView.Application;
public sealed record ScreenViewState(string ScreenId, ScreenKind Kind, string Title, bool IsEnabled, bool IsVisible);
