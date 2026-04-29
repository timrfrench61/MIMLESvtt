namespace VttMvuView.Application;
public sealed record ViewAppModel(ShellViewState Shell, NavigationViewState Navigation, IReadOnlyList<ScreenViewState> Screens, string ActiveScreenId);
