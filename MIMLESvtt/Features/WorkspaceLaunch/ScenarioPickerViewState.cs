namespace VttMvuView.WorkspaceLaunch;
public sealed record ScenarioPickerViewState(IReadOnlyList<ScenarioCardViewModel> Scenarios, string? SelectedScenarioId, ScenarioPickerMode Mode, bool IsLoading);
