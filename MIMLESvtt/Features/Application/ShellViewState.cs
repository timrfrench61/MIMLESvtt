namespace VttMvuView.Application;
public sealed record ShellViewState(bool IsBusy, string? BusyMessage, bool IsLeftRailOpen, bool IsRightInspectorOpen, bool IsStatusBarVisible);
