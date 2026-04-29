namespace VttMvuView.WorkspaceLaunch;
public sealed record GameSystemPickerViewState(IReadOnlyList<GameSystemCardViewModel> AvailableGameSystems, string? SelectedGameSystemId, bool IsLoading);
